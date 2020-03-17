using System.Net;
using System.Net.Sockets;

public class TCPClient 
{
    private bool m_IsRunning = false;

    private Socket m_SystemSocket;

    private IPEndPoint m_remoteEndPoint;

    public TCPClient(string host,int port) {
        m_SystemSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
        m_remoteEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
        m_SystemSocket.Connect(m_remoteEndPoint);
    }

}
