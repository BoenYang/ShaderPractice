using System;
using System.Text;
using System.Threading;

namespace GameServer
{
    public class GameServer
    {
        private KCPServer m_kcp;


        private int m_bindPort;

        public GameServer(int port)
        {
            m_bindPort = port;
        }

        public void Start()
        {
            m_kcp = new KCPServer(m_bindPort);
            m_kcp.AddReceiveListener(this.OnRecivedData);
        }

        private void OnRecivedData(KCPProxy proxy, byte[] data, int size) {

            string content = Encoding.UTF8.GetString(data);
            Console.WriteLine("[Main] Recived Content " + content);
            string ret = "server echo " + content;
            byte[] buffer = Encoding.UTF8.GetBytes(ret);
            proxy.DoSend(buffer, buffer.Length);
        }

        public void Stop()
        {
            m_kcp.Dispose();
        }

        public void Update()
        { 
            m_kcp.Update();
        }
    }
}