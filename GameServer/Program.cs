using System.Threading;

namespace GameServer
{
    class Program
    {
        private GameServer gameServer;

        static void Main(string[] args) {
           Program p = new Program();
           p.Init();
           p.Run();
        }

        public void Init()
        {
            gameServer = new GameServer(1001);
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
