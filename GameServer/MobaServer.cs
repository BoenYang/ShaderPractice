using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NetworkProtocal;
using ProtoBuf;

namespace GameServer
{
    public class MobaServer
    {
        private KCPServer m_kcp;

        private int m_bindPort;

        private Dictionary<uint, ClientSession> sessionDict;

        private MobaGame m_game;

        private long m_lastTicks = 0;

        private int serverFrameInterval = 100;

        public MobaServer(int port)
        {
            m_bindPort = port;
            sessionDict = new Dictionary<uint, ClientSession>();
            m_game = new MobaGame();
        }

        private ClientSession GetSession(uint sid)
        {
            if (!sessionDict.ContainsKey(sid)) {
               
                return null;
            }
            else
            {
                return sessionDict[sid];
            }
        }

        private ClientSession CreateSession(uint sid)
        {
            ClientSession session = new ClientSession(sid);
            sessionDict.Add(sid, session);
            return session;
        }

        public void Start()
        {
            m_kcp = new KCPServer(m_bindPort);
            m_kcp.AddReceiveListener(this.OnRecivedData);
        }

        private void OnRecivedData(KCPProxy proxy, byte[] data, int size) {
            if (size >= MessageHeader.Length) {

                NetMessage msg = new NetMessage();
                msg.Deserialize(data, size);

                uint sid = msg.Header.Sid;
                ClientSession session = GetSession(sid);
                if (session == null)
                {
                    session = CreateSession(sid);
                    m_game.CreatePlayer(session);
                }
                session.BindProxy(proxy);
                session.DoRecieve(msg);
            }
            else
            {
                string content = Encoding.UTF8.GetString(data);
                Console.WriteLine("[Main] Recived Content " + content);
                string ret = "server echo " + content;
                byte[] buffer = Encoding.UTF8.GetBytes(ret);
                proxy.DoSend(buffer, buffer.Length);
            }
        }

        public void Stop()
        {
            m_kcp.Dispose();
        }

        public void Update()
        { 
            m_kcp.Update();
            if (m_game != null)
            {

                long nowticks = DateTime.Now.Ticks;
                long interval = nowticks - m_lastTicks;
                long frameIntervalTicks = serverFrameInterval * 10000;
                if (interval > frameIntervalTicks)
                {
                    m_game.Update();
                    m_lastTicks = nowticks;
                }
            }
        }
    }
}