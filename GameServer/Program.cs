using System;
using System.Security.Cryptography.X509Certificates;
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
            m_kcp = new KCPServer(1001, 1);
            m_kcp.AddReceiveListener(this.OnRecivedData);
        }

        private void OnRecivedData(byte[] data, int size)
        {

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
