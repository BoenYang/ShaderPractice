using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class KCPServer
{
    private static readonly System.DateTime UTCTimeBegin = new DateTime(1970, 1, 1);

    public static UInt32 GetClockMS() {
        return (UInt32)(Convert.ToInt64(DateTime.UtcNow.Subtract(UTCTimeBegin).TotalMilliseconds) & 0xffffffff);
    }

    public delegate void KCPReceiveListener(KCPProxy session, byte[] buff, int size);

    public string LOG_TAG = "KCPSocket";

    private bool m_IsRunning = false;
    private Socket m_SystemSocket;
    private IPEndPoint m_LocalEndPoint;
    private AddressFamily m_AddrFamily;
    private Thread m_ThreadRecv;
    private byte[] m_RecvBufferTemp = new byte[4096];

    //KCP参数
    private List<KCPProxy> m_ListKcp;
    private KCPReceiveListener m_AnyEPListener;

    //=================================================================================
    #region 构造和析构

    public KCPServer(int bindPort, AddressFamily family = AddressFamily.InterNetwork) {

        m_AddrFamily = family;
        m_ListKcp = new List<KCPProxy>();

        m_SystemSocket = new Socket(m_AddrFamily, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, bindPort);
        m_SystemSocket.Bind(ipep);

        m_IsRunning = true;
        m_ThreadRecv = new Thread(Thread_Recv) { IsBackground = true };
        m_ThreadRecv.Start();

    }


    public void Dispose() {
        m_IsRunning = false;
        m_AnyEPListener = null;

        if (m_ThreadRecv != null) {
            m_ThreadRecv.Interrupt();
            m_ThreadRecv = null;
        }

        int cnt = m_ListKcp.Count;
        for (int i = 0; i < cnt; i++) {
            m_ListKcp[i].Dispose();
        }
        m_ListKcp.Clear();

        if (m_SystemSocket != null) {
            try {
                m_SystemSocket.Shutdown(SocketShutdown.Both);

            }
            catch (Exception e) {
                this.LogWarning("Close() " + e.Message + e.StackTrace);
            }

            m_SystemSocket.Close();
            m_SystemSocket = null;
        }
    }

    public void Connect(IPEndPoint remotePoint) {
        m_SystemSocket.Connect(remotePoint);
    }

    public int SelfPort
    {
        get { return (m_SystemSocket.LocalEndPoint as IPEndPoint).Port; }
    }


    public Socket SystemSocket { get { return m_SystemSocket; } }

    #endregion

    //=================================================================================

    public bool EnableBroadcast
    {
        get { return m_SystemSocket.EnableBroadcast; }
        set { m_SystemSocket.EnableBroadcast = value; }
    }

    //=================================================================================
    #region 管理KCP

    private bool IsValidKcpIPEndPoint(IPEndPoint ipep) {
        if (ipep == null || ipep.Port == 0 ||
            ipep.Address.Equals(IPAddress.Any) ||
            ipep.Address.Equals(IPAddress.IPv6Any)) {
            return false;
        }
        return true;
    }

    private KCPProxy GetKcp(uint sid) {
        int cnt = m_ListKcp.Count;
        for (int i = 0; i < cnt; i++) {
            if (m_ListKcp[i].sid == sid) {
                return m_ListKcp[i];
            }
        }
        return null;
    }

    private KCPProxy CreateKcp(uint sid, IPEndPoint ipep) {
        KCPProxy proxy = new KCPProxy(sid, ipep, m_SystemSocket);
        proxy.AddReceiveListener(OnReceiveAny);
        m_ListKcp.Add(proxy);
        return proxy;
    }


    public void CloseKcp(IPEndPoint ipep) {
        KCPProxy proxy = null;
        int cnt = m_ListKcp.Count;
        for (int i = 0; i < cnt; i++) {
            proxy = m_ListKcp[i];
            if (proxy.RemotePoint.Equals(ipep)) {
                m_ListKcp.RemoveAt(i);
                break;
            }
        }
    }

    #endregion

    //=================================================================================
    #region 主线程驱动

    public void Update() {
        if (m_IsRunning) {
            //获取时钟
            uint current = GetClockMS();

            int cnt = m_ListKcp.Count;
            for (int i = 0; i < cnt; i++) {
                KCPProxy proxy = m_ListKcp[i];
                proxy.Update(current);
            }
        }
    }

    #endregion

    //=================================================================================
    #region 接收逻辑

    public void AddReceiveListener(KCPReceiveListener listener) {
        m_AnyEPListener += listener;
    }

    public void RemoveReceiveListener(KCPReceiveListener listener) {
        m_AnyEPListener -= listener;
    }


    private void OnReceiveAny(KCPProxy proxy, byte[] buffer, int size) {
        if (m_AnyEPListener != null) {
            m_AnyEPListener(proxy, buffer, size);
        }
    }

    #endregion

    //=================================================================================
    #region 接收线程

    private void Thread_Recv() {
        this.Log("Thread_Recv() Begin ......");

        while (m_IsRunning) {
            try {
                DoReceive();
            }
            catch (Exception e) {
                this.LogWarning("Thread_Recv() " + e.Message + "\n" + e.StackTrace);
                Thread.Sleep(10);
            }
        }

        this.Log("Thread_Recv() End!");
    }

    private void DoReceive() {
        EndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
        int cnt = m_SystemSocket.ReceiveFrom(m_RecvBufferTemp, m_RecvBufferTemp.Length,
            SocketFlags.None, ref remotePoint);

        if (cnt > 0) {
            if (cnt >= 4) {
                byte[] m_32b = new byte[4];
                Buffer.BlockCopy(m_RecvBufferTemp, 0, m_32b, 0, 4);
                uint sid = BitConverter.ToUInt32(m_32b, 0);
                KCPProxy proxy = GetKcp(sid);
                if (proxy == null) {
                    proxy = CreateKcp(sid, (IPEndPoint)remotePoint);
                }
                proxy.DoReceiveInThread(m_RecvBufferTemp, cnt);
            }

        }

    }

    private void Log(string log) {
    }

    private void LogWarning(string log) {
    }

    #endregion
}

public class KCPProxy
{

    private KCP m_Kcp;
    private bool m_NeedKcpUpdateFlag = false;
    private uint m_NextKcpUpdateTime = 0;
    private SwitchQueue<byte[]> m_RecvQueue = new SwitchQueue<byte[]>(128);

    private IPEndPoint m_RemotePoint;
    private Socket m_Socket;
    private KCPServer.KCPReceiveListener m_Listener;

    public IPEndPoint RemotePoint { get { return m_RemotePoint; } }

    private uint m_sid;

    public uint sid
    {
        get => m_sid;
    }

    public KCPProxy(uint key, IPEndPoint remotePoint, Socket socket) {
        m_Socket = socket;
        m_RemotePoint = remotePoint;

        m_sid = key;

        m_Kcp = new KCP(m_sid, HandleKcpSend);
        m_Kcp.NoDelay(1, 10, 2, 1);
        m_Kcp.WndSize(128, 128);

    }

    public void Dispose() {
        m_Socket = null;

        if (m_Kcp != null) {
            m_Kcp.Dispose();
            m_Kcp = null;
        }

        m_Listener = null;
    }

    //---------------------------------------------
    private void HandleKcpSend(byte[] buff, int size) {
        if (m_Socket != null) {
            m_Socket.SendTo(buff, 0, size, SocketFlags.None, m_RemotePoint);
        }
    }

    private void HandleKcpSend_Hook(byte[] buff, int size) {
        if (m_Socket != null) {
            m_Socket.SendTo(buff, 0, size, SocketFlags.None, m_RemotePoint);
        }
    }

    public bool DoSend(byte[] buff, int size) {
        m_NeedKcpUpdateFlag = true;
        return m_Kcp.Send(buff, size) >= 0;
    }

    //---------------------------------------------

    public void AddReceiveListener(KCPServer.KCPReceiveListener listener) {
        m_Listener += listener;
    }

    public void RemoveReceiveListener(KCPServer.KCPReceiveListener listener) {
        m_Listener -= listener;
    }



    public void DoReceiveInThread(byte[] buffer, int size) {
        byte[] dst = new byte[size];
        Buffer.BlockCopy(buffer, 0, dst, 0, size);
        m_RecvQueue.Push(dst);
    }

    private void HandleRecvQueue() {
        m_RecvQueue.Switch();
        while (!m_RecvQueue.Empty()) {
            var recvBufferRaw = m_RecvQueue.Pop();
            int ret = m_Kcp.Input(recvBufferRaw);

            //收到的不是一个正确的KCP包
            if (ret < 0) {
                //if (m_Listener != null) {
                //    m_Listener(this,recvBufferRaw, recvBufferRaw.Length);
                //}
                return;
            }

            m_NeedKcpUpdateFlag = true;
            KCP kcp = m_Kcp;

            for (int size = kcp.PeekSize(); size > 0; size = kcp.PeekSize()) {
                var recvBuffer = new byte[size];
                if (kcp.Recv(recvBuffer) > 0) {
                    if (m_Listener != null) {
                        m_Listener(this, recvBuffer, size);
                    }
                }
            }
        }
    }

    //---------------------------------------------
    public void Update(uint currentTimeMS) {
        HandleRecvQueue();

        if (m_NeedKcpUpdateFlag || currentTimeMS >= m_NextKcpUpdateTime) {
            if (m_Kcp != null) {
                m_Kcp.Update(currentTimeMS);
                m_NextKcpUpdateTime = m_Kcp.Check(currentTimeMS);
                m_NeedKcpUpdateFlag = false;
            }
        }
    }

    //---------------------------------------------

}