using System;
using NetworkProtocal;

namespace GameServer
{
    public class MobaPlayer
    {
        private uint m_id;

        private ClientSession m_session;

        private Action<MobaPlayer, NetMessage> m_Listener;

        private PlayerInfo m_userInfo;

        public PlayerInfo PlayerInfo
        {
            get => m_userInfo;
            set
            {
                if (value != null) m_userInfo = value;
            }
        }

        public uint Id
        {
            get => m_id;
        }

    
        public float posX;

        public float posY;

        public float posZ;

        public uint teamId;

        private PlayerGameInfo m_GameInfo = new PlayerGameInfo();

        public PlayerGameInfo GameInfo
        {
            get
            {
                m_GameInfo.PosX = (int)posX * 100;
                m_GameInfo.PosY = (int)posY * 100;
                m_GameInfo.PosZ = (int)posZ * 100;
                return m_GameInfo;
            }
        }

        public void InitGameInfo(uint teamId)
        {
            posX = 0;
            posY = 0;
            posZ = 0;
            this.teamId = teamId;
            m_GameInfo.HeroId = 1;
            m_GameInfo.PlayerId = Id;
            m_GameInfo.TeamId = this.teamId;
        }

        public MobaPlayer(ClientSession session,Action<MobaPlayer,NetMessage> msgListener)
        {
            this.m_id = session.Sid;
            m_session = session;
            m_session.AddNetMessageListener(this.OnRecivedNetMessage);
            m_Listener = msgListener;
            m_userInfo = new PlayerInfo();
        }

        private void OnRecivedNetMessage(NetMessage msg)
        {
            if (m_Listener != null)
            {
                m_Listener(this, msg);
            }
        }

        public void Send<T>(GameCmd cmd, T t)
        {
            byte[] protoBytes = PBUtils.PBSerialize(t);
            NetMessage msg = new NetMessage();
            msg.Header.Sid = 0;
            msg.Header.MessageId = (uint)cmd;
            msg.Header.dataSize = (ushort)protoBytes.Length;
            msg.Header.TimeStamp = GetTimeStamp();
            msg.Data = protoBytes;

            byte[] buffer = null;
            int len = msg.Serialize(out buffer);
            if (buffer == null) {
                return;
            }
            m_session.Send(buffer, len);
        }

        public void SetReady(bool ready)
        {
            PlayerInfo.ready = ready;
        }
        

        public void Update()
        {

        }

        double GetTimeStamp() {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return ts.TotalMilliseconds;
        }


    }
}