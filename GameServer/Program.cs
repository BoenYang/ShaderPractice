using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace GameServer
{
    class Program
    {
        private KCPServer m_kcp;

        static void Main(string[] args) {
           Program p = new Program();
           p.Init();
           p.Run();
        }

        public void Init()
        {
            m_kcp = new KCPServer(1001);
            m_kcp.AddReceiveListener(this.OnRecivedData);
        }

        private void OnRecivedData(KCPProxy proxy,byte[] data, int size)
        {
            string content = Encoding.UTF8.GetString(data);
            Console.WriteLine("[Main] Recived Content " + content);
            string ret = "server echo " + content;
            byte[] buffer = Encoding.UTF8.GetBytes(ret);
            proxy.DoSend(buffer, buffer.Length);
        }

        public void Run()
        {
            while (true)
            {
                m_kcp.Update();
                Thread.Sleep(1);
            }
        }
    }
}
