using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class KCPClient
{

    private bool m_IsRunning = false;
    
    private Socket m_SystemSocket;
    
    private uint m_KcpKey;

    private Thread m_ThreadRecv;

    private KCP m_kcp;

    private IPEndPoint m_remoteEndPoint;

    private bool m_NeedKcpUpdateFlag = false;

    private uint m_NextKcpUpdateTime = 0;

    private static readonly DateTime UTCTimeBegin = new DateTime(1970, 1, 1);

    private byte[] m_RecvBufferTemp = new byte[4096];

    private SwitchQueue<byte[]> m_RecvQueue = new SwitchQueue<byte[]>(128);

    public delegate void KCPReceiveListener(byte[] buff, int size);

    public event KCPReceiveListener m_Listener;

    private static UInt32 GetClockMS() {
        return (UInt32)(Convert.ToInt64(DateTime.UtcNow.Subtract(UTCTimeBegin).TotalMilliseconds) & 0xffffffff);
    }

    public KCPClient(string host,int hostPort, int bindPort, uint kcpKey) {
        m_KcpKey = kcpKey;

        m_SystemSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, bindPort);
        m_SystemSocket.Bind(ipep);

        m_kcp = new KCP(m_KcpKey, kcpSend);

        m_remoteEndPoint = new IPEndPoint(IPAddress.Parse(host), hostPort);

        m_IsRunning = true;
        m_ThreadRecv = new Thread(Thread_Recv) { IsBackground = true };
        m_ThreadRecv.Start();
    }

    public void Close() { 
    
    }


    public void Send(byte[] buffer, int size) {
        m_kcp.Send(buffer, size);
    }

    public void Update() {
        if (m_IsRunning) {
            m_RecvQueue.Switch();
            while (!m_RecvQueue.Empty()) {
                var recvBufferRaw = m_RecvQueue.Pop();

                int ret = m_kcp.Input(recvBufferRaw);

                //收到的不是一个正确的KCP包
                if (ret < 0) {
                    if (m_Listener != null) {
                        m_Listener(recvBufferRaw, recvBufferRaw.Length);
                    }
                    return;
                }

                m_NeedKcpUpdateFlag = true;

                for (int size = m_kcp.PeekSize(); size > 0; size = m_kcp.PeekSize()) {
                    var recvBuffer = new byte[size];
                    if (m_kcp.Recv(recvBuffer) > 0) {
                        if (m_Listener != null) {
                            m_Listener(recvBuffer, size);
                        }
                    }
                }
            }

            if (m_IsRunning) {

                //获取时钟
                uint currentTimeMS = GetClockMS();
                if (m_NeedKcpUpdateFlag || currentTimeMS >= m_NextKcpUpdateTime) {
                    if (m_kcp != null) {
                        m_kcp.Update(currentTimeMS);
                        m_NextKcpUpdateTime = m_kcp.Check(currentTimeMS);
                        m_NeedKcpUpdateFlag = false;
                    }
                }
            }

        }
    }

    private void kcpSend(byte[] buffer, int size) {
        if (m_SystemSocket != null) {
            m_SystemSocket.SendTo(buffer, 0, size, SocketFlags.None, m_remoteEndPoint);
        }
    }

    private void Thread_Recv() {
        while (m_IsRunning) {
            try {
                DoReceive();
            }
            catch (Exception e) {
                Thread.Sleep(10);
            }
        }

    }

    private void DoReceive() {
        EndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
        int cnt = m_SystemSocket.ReceiveFrom(m_RecvBufferTemp, m_RecvBufferTemp.Length,
            SocketFlags.None, ref remotePoint);

        if (cnt > 0) {
            byte[] dst = new byte[cnt];
            Buffer.BlockCopy(m_RecvBufferTemp, 0, dst, 0, cnt);
            m_RecvQueue.Push(dst);
        }
    }
}
