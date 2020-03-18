using System;
using System.Text;

namespace GameServer
{
    public class ByteBuffer
    {
        public static byte[] ReverseOrder(byte[] dt) {
            if (!BitConverter.IsLittleEndian) {
                return dt;
            }

            int count = dt.Length;

            if (count > 1) {
                int half = count / 2;
                int maxIndex = count - 1;

                for (int i = 0; i < half; i++) {
                    byte temp = dt[i];
                    int swapIndex = maxIndex - i;

                    dt[i] = dt[swapIndex];
                    dt[swapIndex] = temp;
                }
            }

            return dt;
        }

        public int Capacity => m_capacity;
        public int Length => m_len;

        public int Position
        {
            get => m_pos;
            set => m_pos = value;
        }

        public int BytesAvailable { get { return (m_len - m_pos); } }

        protected int m_pos = 0;
        protected int m_len = 0;
        protected int m_capacity = 0;
        protected byte[] m_buff = null;

        protected byte[] m_16b = new byte[2];
        protected byte[] m_32b = new byte[4];
        protected byte[] m_64b = new byte[8];

        public ByteBuffer(int capacity) {
            this.m_capacity = capacity;
            this.m_buff = new byte[capacity];
            this.m_pos = 0;
            this.m_len = 0;
        }

        public ByteBuffer(byte[] buff = null) {
            this.m_capacity = 0;
            this.m_pos = 0;
            this.m_len = 0;
            this.m_buff = buff;
            if (buff != null) {
                this.m_capacity = buff.Length;
            }
        }

        public ByteBuffer Attach(byte[] buff, int len) {
            if (buff == null) {
                throw new Exception("NetBuffer Attach A Null Buffer!");
            }

            this.m_pos = 0;
            this.m_buff = buff;
            this.m_len = System.Math.Min(len, buff.Length);
            this.m_capacity = buff.Length;
            return this;
        }

        public void Clear() {
            m_len = 0;
            m_pos = 0;
        }

        public void AdjustCapacity(int newCapacity) {
            if (newCapacity < m_len) {
                m_len = newCapacity;
                if (m_pos > m_len) {
                    m_pos = m_len;
                }
            }

            byte[] newBuff = new byte[newCapacity];
            Buffer.BlockCopy(m_buff, 0, newBuff, 0, m_len);

            m_capacity = newCapacity;
            m_buff = newBuff;
        }

        public byte[] GetBytes() {
            return m_buff;
        }

        public void SetPositionToLength() {
            m_pos = m_len;
        }


        public void Arrangement(int pos = -1) {
            pos = pos == -1 ? m_pos : pos;
            if (pos > m_len) {
                pos = m_len;
            }

            if (pos < 0) {
                pos = 0;
            }

            int size = 0;
            if (pos < m_len) {
                size = m_len - pos;
            }

            Buffer.BlockCopy(m_buff, pos, m_buff, 0, size);
            m_len = size;
            m_pos = 0;
        }

        private int UpdateLenAndGetWritePos(int writePos, int writeLen) {
            if (writePos == -1) {
                if (m_len + writeLen > m_capacity) {
                    throw new Exception("ByteBuffer out of capacity.");
                }
                writePos = m_len;
                m_len += writeLen;
            }
            else {
                if (writePos + writeLen > m_capacity) {
                    throw new Exception("ByteBuffer out of capacity.");
                }
                if (writePos + writeLen > m_len) {
                    m_len += writeLen;
                }
            }

            return writePos;
        }

        public int AddBytes(byte[] src, int srcOffset = 0, int count = 0) {
            if (count <= 0) {
                count = src.Length - srcOffset;
            }
            if (m_len + count > m_capacity) {
                throw new Exception("ByteBuffer(" + m_len + "+" + count + ") Out of Capacity(" + m_capacity + ").");
            }

            Buffer.BlockCopy(src, srcOffset, m_buff, m_len, count);
            m_len += count;
            return m_len;
        }

        public int AddBuffer(ByteBuffer src) {
            return AddBytes(src.m_buff, 0, src.m_len);
        }

        public int CopyWith(ByteBuffer src, int dstOffset = 0, bool bResetLen = false) {
            return CopyWith(src, 0, dstOffset, bResetLen);
        }

        public int CopyWith(ByteBuffer src, int srcOffset, int dstOffset = 0, bool bResetLen = false) {
            Buffer.BlockCopy(src.m_buff, srcOffset, m_buff, dstOffset, src.m_len);

            int newLen = dstOffset + src.m_len - srcOffset;
            if (newLen > m_len || bResetLen) {
                m_len = newLen;
            }
            return m_len;
        }

        public void AddLength(int len, int writePos = -1) {
            UpdateLenAndGetWritePos(writePos, len);
            if (writePos == 0) {
                m_len = len;
            }
        }

        public int WriteByte(byte value, int writePos = -1) {
            int pos = UpdateLenAndGetWritePos(writePos, 1);
            m_buff[pos] = value;
            return pos + 1;
        }

        public int WriteShort(short value, int writePos = -1) {
            int pos = UpdateLenAndGetWritePos(writePos, 2);
            m_buff[pos + 0] = (byte)(value >> 8 & 0xFF);
            m_buff[pos + 1] = (byte)(value >> 0 & 0xFF);
            return pos + 2;
        }
        public int WriteInt(int value, int writePos = -1) {
            int pos = UpdateLenAndGetWritePos(writePos, 4);
            m_buff[pos + 0] = (byte)(value >> 24 & 0xFF);
            m_buff[pos + 1] = (byte)(value >> 16 & 0xFF);
            m_buff[pos + 2] = (byte)(value >> 08 & 0xFF);
            m_buff[pos + 3] = (byte)(value >> 00 & 0xFF);
            return pos + 4;
        }

        public int WriteUShort(ushort value, int writePos = -1) {
            return WriteShort((short)value, writePos);
        }

        public int WriteUInt(uint value, int writePos = -1) {
            return WriteInt((int)value, writePos);
        }

        public int WriteBytes(byte[] src, int srcOffset, int count, int writePos = -1) {
            int pos = UpdateLenAndGetWritePos(writePos, count);
            Buffer.BlockCopy(src, srcOffset, m_buff, pos, count);
            return pos + count;
        }


        public int WriteBytes(byte[] value, int writePos = -1) {
            return WriteBytes(value, 0, value.Length, writePos);
        }

        public int WriteDouble(double value, int writePos = -1) {
            byte[] bytes = BitConverter.GetBytes(value);
            return WriteBytes(ByteBuffer.ReverseOrder(bytes), writePos);
        }

        public int WriteFloat(float value, int writePos = -1) {
            byte[] bytes = BitConverter.GetBytes(value);
            return WriteBytes(ByteBuffer.ReverseOrder(bytes), writePos);
        }

        public void WriteLong(long l, int writePos = -1) {
            byte[] bytes = BitConverter.GetBytes(l);
            this.WriteBytes(ByteBuffer.ReverseOrder(bytes), writePos);
        }

        public void WriteULong(ulong l, int writePos = -1) {
            byte[] bytes = BitConverter.GetBytes(l);
            this.WriteBytes(ByteBuffer.ReverseOrder(bytes), writePos);
        }

        public int WriteUTF8(string value, int writePos = -1) {
            byte[] tmp = Encoding.UTF8.GetBytes(value);
            int nextPos = WriteInt(tmp.Length, writePos);
            return WriteBytes(tmp, nextPos);
        }

        public int Skip(int count) {
            m_pos += count;
            return m_pos;
        }

        public void ReadBytes(byte[] dst, int dstOffset, int count) {
            Buffer.BlockCopy(this.m_buff, this.m_pos, dst, dstOffset, count);
            this.m_pos += count;
        }

        public byte[] ReadBytes(int count) {
            byte[] dst = new byte[count];
            ReadBytes(dst, 0, count);
            return dst;
        }

        public bool ReadBool() {
            return (this.m_buff[this.m_pos++] == 1);
        }

        public byte ReadByte() {
            return this.m_buff[this.m_pos++];
        }

        public double ReadDouble() {
            ReadBytes(m_64b, 0, 8);
            return BitConverter.ToDouble(ByteBuffer.ReverseOrder(m_64b), 0);
        }

        public float ReadFloat() {
            ReadBytes(m_32b, 0, 4);
            return BitConverter.ToSingle(ByteBuffer.ReverseOrder(m_32b), 0);
        }

        public int ReadInt() {
            ReadBytes(m_32b, 0, 4);
            return BitConverter.ToInt32(ByteBuffer.ReverseOrder(m_32b), 0);
        }

        public uint ReadUInt() {
            ReadBytes(m_32b, 0, 4);
            return BitConverter.ToUInt32(ByteBuffer.ReverseOrder(m_32b), 0);
        }

        public long ReadLong() {
            ReadBytes(m_64b, 0, 8);
            return BitConverter.ToInt64(ByteBuffer.ReverseOrder(m_64b), 0);
        }

        public ulong ReadULong() {
            ReadBytes(m_64b, 0, 8);
            return BitConverter.ToUInt64(ByteBuffer.ReverseOrder(m_64b), 0);
        }

        public short ReadShort() {
            ReadBytes(m_16b, 0, 2);
            return BitConverter.ToInt16(ByteBuffer.ReverseOrder(m_16b), 0);
        }

        public ushort ReadUShort() {
            ReadBytes(m_16b, 0, 2);
            return BitConverter.ToUInt16(ByteBuffer.ReverseOrder(m_16b), 0);
        }

        public string ReadString() {
            int count = this.ReadInt();
            string str = Encoding.UTF8.GetString(this.m_buff, this.m_pos, count);
            this.m_pos += count;
            return str;
        }

        public override string ToString() {
            return "";
        }
    }
}