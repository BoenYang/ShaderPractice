
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class KCPServer
{
    private static readonly System.DateTime UTCTimeBegin = new DateTime(1970, 1, 1);

    public static UInt32 GetClockMS() {
        return (UInt32)(Convert.ToInt64(DateTime.UtcNow.Subtract(UTCTimeBegin).TotalMilliseconds) & 0xffffffff);
    }

    public delegate void KCPReceiveListener(byte[] buff, int size, IPEndPoint remotePoint);



    public string LOG_TAG = "KCPSocket";

    private bool m_IsRunning = false;
    private Socket m_SystemSocket;
    private IPEndPoint m_LocalEndPoint;
    private AddressFamily m_AddrFamily;
    private Thread m_ThreadRecv;
    private byte[] m_RecvBufferTemp = new byte[4096];

    //KCP参数
    private List<KCPProxy> m_ListKcp;
    private uint m_KcpKey = 0;
    private KCPReceiveListener m_AnyEPListener;

    //=================================================================================
    #region 构造和析构

    public KCPServer(int bindPort, uint kcpKey, AddressFamily family = AddressFamily.InterNetwork) {

        m_AddrFamily = family;
        m_KcpKey = kcpKey;
        m_ListKcp = new List<KCPProxy>();

        m_SystemSocket = new Socket(m_AddrFamily, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, bindPort);
        m_SystemSocket.Bind(ipep);

        bindPort = (m_SystemSocket.LocalEndPoint as IPEndPoint).Port;
        LOG_TAG = "KCPSocket[" + bindPort + "-" + kcpKey + "]";

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

    private KCPProxy GetKcp(IPEndPoint ipep, bool autoAdd = true) {
        if (!IsValidKcpIPEndPoint(ipep)) {
            return null;
        }

        KCPProxy proxy = null;
        int cnt = m_ListKcp.Count;
        for (int i = 0; i < cnt; i++) {
            proxy = m_ListKcp[i];
            if (proxy.RemotePoint.Equals(ipep)) {
                return proxy;
            }
        }

        if (autoAdd) {
            proxy = new KCPProxy(m_KcpKey, ipep, m_SystemSocket);
            proxy.AddReceiveListener(OnReceiveAny);
            m_ListKcp.Add(proxy);
        }
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
    #region 发送逻辑
    public bool SendTo(byte[] buffer, int size, IPEndPoint remotePoint) {
        if (remotePoint.Address == IPAddress.Broadcast) {
            int cnt = m_SystemSocket.SendTo(buffer, size, SocketFlags.None, remotePoint);
            return cnt > 0;
        }
        else {
            KCPProxy proxy = GetKcp(remotePoint);
            if (proxy != null) {
                return proxy.DoSend(buffer, size);
            }
        }

        return false;
    }

    public bool SendTo(string message, IPEndPoint remotePoint) {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        return SendTo(buffer, buffer.Length, remotePoint);
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

    public void AddReceiveListener(IPEndPoint remotePoint, KCPReceiveListener listener) {
        KCPProxy proxy = GetKcp(remotePoint);
        if (proxy != null) {
            proxy.AddReceiveListener(listener);
        }
        else {
            m_AnyEPListener += listener;
        }
    }

    public void RemoveReceiveListener(IPEndPoint remotePoint, KCPReceiveListener listener) {
        KCPProxy proxy = GetKcp(remotePoint);
        if (proxy != null) {
            proxy.RemoveReceiveListener(listener);
        }
        else {
            m_AnyEPListener -= listener;
        }
    }

    public void AddReceiveListener(KCPReceiveListener listener) {
        m_AnyEPListener += listener;
    }

    public void RemoveReceiveListener(KCPReceiveListener listener) {
        m_AnyEPListener -= listener;
    }


    private void OnReceiveAny(byte[] buffer, int size, IPEndPoint remotePoint) {
        if (m_AnyEPListener != null) {
            m_AnyEPListener(buffer, size, remotePoint);
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
            KCPProxy proxy = GetKcp((IPEndPoint)remotePoint);
            if (proxy != null) {
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
