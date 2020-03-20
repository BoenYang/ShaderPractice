using System.Threading;

namespace GameServer
{
    class Program
    {
        private MobaServer gameServer;

        static void Main(string[] args) {
           Program p = new Program();
           p.Init();
           p.Run();
        }

        public void Init()
        {
            gameServer = new MobaServer(1001);
            gameServer.Start();
        }

        public void Run()
        {
            while (true)
            {
                gameServer.Update();
                Thread.Sleep(1);
            }
        }
    }
}
