using System;
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

        [ProtoMember(3)]
        public bool ready;

    }

    [ProtoContract]
    public class EnterRoomRequest
    {
        [ProtoMember(1)]
        public uint id;

        [ProtoMember(2)] 
        public string name;
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
    public class PlayerInitedReqeust
    {

    }

    [ProtoContract]
    public class PlayerInitedRespnose
    {

    }

    [ProtoContract]
    public class PlayerGameInfo
    {
        [ProtoMember(1,IsRequired = true)] 
        public uint PlayerId;
        [ProtoMember(2, IsRequired = true)]
        public uint HeroId;
        [ProtoMember(3, IsRequired = true)]
        public uint TeamId;
        [ProtoMember(4, IsRequired = true)]
        public int PosX;
        [ProtoMember(5, IsRequired = true)]
        public int PosY;
        [ProtoMember(6, IsRequired = true)]
        public int PosZ;
        [ProtoMember(7, IsRequired = true)]
        public int RotX;
        [ProtoMember(8, IsRequired = true)]
        public int RotY;
        [ProtoMember(9, IsRequired = true)]
        public int RotZ;
        [ProtoMember(10, IsRequired = true)]
        public int MoveX;
        [ProtoMember(11, IsRequired = true)] 
        public int MoveZ;
    }

}