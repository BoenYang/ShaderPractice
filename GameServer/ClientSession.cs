using System;

namespace GameServer
{
    public class ClientSession
    {
        private KCPProxy m_proxy;

        private uint m_sid;

        public uint Sid => m_sid;

        private bool m_active;

        private Action<NetMessage> m_listener;

        public ClientSession(uint sid)
        {
            m_sid = sid;
        }

        public void DoRecieve(NetMessage msg)
        {
            if (this.m_listener != null)
            {
                this.m_listener(msg);
            }
        }

        public void AddNetMessageListener(Action<NetMessage> listener)
        {
            m_listener = listener;
        }

        public void BindProxy(KCPProxy proxy)
        {
            m_proxy = proxy;
        }

        public void Send(byte[] data, int size)
        {
            m_proxy.DoSend(data, size);
        }

        public void Close()
        {

        }
    }
   
}