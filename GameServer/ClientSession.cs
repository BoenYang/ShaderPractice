namespace GameServer
{
    public class ClientSession
    {
        private KCPProxy m_proxy;

        private uint m_sid;

        public uint Sid => m_sid;

        private MobaPlayer m_player;

        public ClientSession(uint sid)
        {
            m_sid = sid;
        }


        public void BindProxy(KCPProxy proxy)
        {
            m_proxy = proxy;
        }

        public void BindPlayer(MobaPlayer player) {

        }
    }
   
}