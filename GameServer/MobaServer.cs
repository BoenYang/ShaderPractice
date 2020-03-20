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

        public MobaServer(int port)
        {
            m_bindPort = port;
            sessionDict = new Dictionary<uint, ClientSession>();
        }

        private ClientSession GetSession(uint sid)
        {
            if (!sessionDict.ContainsKey(sid)) {
                ClientSession session = new ClientSession(sid);
                sessionDict.Add(sid,session);
                return session;
            }
            else
            {
                return sessionDict[sid];
            }
        }

        public void Start()
        {
            m_kcp = new KCPServer(m_bindPort);
            m_kcp.AddReceiveListener(this.OnRecivedData);
        }

        private void OnRecivedData(KCPProxy proxy, byte[] data, int size) {
            if (size > MessageHeader.Length) {

                NetMessage msg = new NetMessage();
                msg.Deserialize(data, size);
                if (msg.Header.MessageId == GameCmd.EnterRoomRequest) {
                    EnterRoomRequest t = default(EnterRoomRequest);
                    using (MemoryStream m = new MemoryStream(msg.Data)) {
                        t = Serializer.Deserialize<EnterRoomRequest>(m);
                    }
                    Console.WriteLine("Enter Room Request " + t.id);
                }
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
        }
    }
}