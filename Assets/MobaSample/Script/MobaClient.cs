using System;
using System.Collections;
using System.Text;
using UnityEngine;


public class MobaClient {

    private KCPClient m_KcpClient;

    private uint m_sid;

    public MobaClient(string host, int port,uint sid)
    {
        m_KcpClient = new KCPClient("127.0.0.1", 1001);
        m_KcpClient.m_Listener += OnRecieveData;
        m_sid = sid;
    }

    public void Send(uint cmd, byte[] buffer,int size)
    {
        NetMessage msg = new NetMessage();
        msg.Header.MessageId = cmd;
        msg.Header.Sid = m_sid;
        msg.Header.dataSize = (ushort)size;
        msg.Header.TimeStamp = GetTimeStamp();
        msg.Data = buffer;

        byte[] data;
        msg.Serialize(out data);
        if (data != null)
        {
            m_KcpClient.Send(data, data.Length);
        }
    }

    void OnRecieveData(byte[] data, int size) {
        string content = Encoding.UTF8.GetString(data);
        Debug.Log(content);
    }

    double GetTimeStamp()
    {
        TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return ts.TotalMilliseconds;
    }

    public void Update()
    {
        if (m_KcpClient != null)
        {
            m_KcpClient.Update();
        }
    }

}
