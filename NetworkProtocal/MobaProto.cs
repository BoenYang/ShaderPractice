using ProtoBuf;

namespace NetworkProtocal
{
    [ProtoContract]
    public class EnterRoomRequest
    {
        [ProtoMember(1)]
        public int id;
    }

    [ProtoContract]
    public class EnterRoomResponse
    {

    }

    [ProtoContract]
    public class ErrorResponse
    {

    }

}