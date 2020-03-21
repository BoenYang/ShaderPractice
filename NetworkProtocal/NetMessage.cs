using System;

public class MessageHeader
{

    public static int Length = 18;

    public uint Sid = 0;
    public uint MessageId = 0;
    public ushort dataSize = 0;
    public double TimeStamp = 0;

    public void Deserialize(ByteBuffer byteBuffer)
    {
        Sid = byteBuffer.ReadUInt();
        MessageId = byteBuffer.ReadUInt();
        dataSize = byteBuffer.ReadUShort();
        TimeStamp = byteBuffer.ReadDouble();
    }

    public void Serialize(ByteBuffer byteBuffer)
    {
        byteBuffer.WriteUInt(Sid);
        byteBuffer.WriteUInt(MessageId);
        byteBuffer.WriteUShort(dataSize);
        byteBuffer.WriteDouble(TimeStamp);
    }
}

public class NetMessage
{
    public MessageHeader Header = new MessageHeader();

    public byte[] Data;

    public void Deserialize(byte[] bytes,int size) {
        ByteBuffer byteBuffer = new ByteBuffer();
        byteBuffer.Attach(bytes,size);
        Header.Deserialize(byteBuffer);
        if (Header.dataSize > 0)
        {
            Data = new byte[Header.dataSize];
            byteBuffer.ReadBytes(Data, 0, Header.dataSize);
        }
    }

    public int Serialize(out byte[] bytes)
    {
        ByteBuffer byteBuffer = new ByteBuffer(Header.dataSize + MessageHeader.Length);
        Header.Serialize(byteBuffer);
        if (Data != null)
        {
            byteBuffer.WriteBytes(Data);
        }
        bytes = byteBuffer.GetBytes();
        return byteBuffer.Length;
    }
}

