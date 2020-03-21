using System.IO;
using ProtoBuf;

namespace NetworkProtocal
{
    public class PBUtils
    {
        public static byte[] PBSerialize<T>(T t)
        {
            if (t == null) return null;

            byte[] buffer = null;
            using (MemoryStream m = new MemoryStream()) {
                Serializer.Serialize<T>(m, t);
                m.Position = 0;
                int length = (int)m.Length;
                buffer = new byte[length];
                m.Read(buffer, 0, length);
            }

            return buffer;
        }

        public static T PBDeserialize<T>(byte[] buffer)
        {
            T proto = default(T);
            using (MemoryStream m = new MemoryStream(buffer)) {
                proto = Serializer.Deserialize<T>(m);
            }
            return proto;
        }
    }
}