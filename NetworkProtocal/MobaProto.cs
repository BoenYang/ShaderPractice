using System.Collections.Generic;
using ProtoBuf;

namespace NetworkProtocal
{
    [ProtoContract]
    public class PlayerInfo
    {
        [ProtoMember(1)]
        public uint id;

        [ProtoMember(2)]
        public string name;

    }

    [ProtoContract]
    public class EnterRoomRequest
    {
        [ProtoMember(1)]
        public PlayerInfo playerInfo;
    }



    [ProtoContract]
    public class EnterRoomResponse
    {
        [ProtoMember(1)]
        public List<PlayerInfo> playerInfos;
    }

    [ProtoContract]
    public class EnterRoomBroadcast
    {
        [ProtoMember(1)]
        public PlayerInfo playerInfo;
    }


    [ProtoContract]
    public class ErrorResponse
    {

        [ProtoMember(1)] 
        public uint errorCode;

        [ProtoMember(2)]
        public string errorMsg;
    }

    [ProtoContract]
    public class ReadyRequest
    {
        [ProtoMember(1)]
        public bool ready;
    }

    [ProtoContract]
    public class ReadyBroadcast
    {

        [ProtoMember(1)]
        public uint playerId;

        [ProtoMember(2)]
        public bool ready;
    }

    [ProtoContract]
    public class EnterSceneBroadcast
    {
        [ProtoMember(1)] 
        public List<PlayerGameInfo> playerGameInfos;
    }

    [ProtoContract]
    public class PlayerGameInfo
    {
        [ProtoMember(1)] 
        public uint PlayerId;
        [ProtoMember(2)]
        public uint ConfigId;
        [ProtoMember(3)]
        public uint TeamId;
        [ProtoMember(4)]
        public int PosX;
        [ProtoMember(5)]
        public int PosY;
        [ProtoMember(6)]
        public int PosZ;
        [ProtoMember(7)]
        public int RotX;
        [ProtoMember(8)]
        public int RotY;
        [ProtoMember(9)]
        public int RotZ;

    }

    [ProtoContract]
    public class PlayerSyncInfo
    {

    }

}