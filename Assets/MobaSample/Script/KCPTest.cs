using System;
using System.Text;
using UnityEngine;

public class KCPTest : MonoBehaviour
{
    private KCPClient client;

    // Start is called before the first frame update
    void Start()
    {
        client = new KCPClient("127.0.0.1",1001);
        client.m_Listener += OnRecieveData;
     
    }

    void OnGUI()
    {
        if (GUILayout.Button("Send", GUILayout.Width(200), GUILayout.Height(60)))
        {
            string content = "hello world " + DateTime.Now;
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            client.Send(buffer, buffer.Length);
            Debug.Log("send data");
        }
    }

    void OnRecieveData(byte[] data, int size)
    {
        string content = Encoding.UTF8.GetString(data);
        Debug.Log(content);
    }

    void Update()
    {
        if (client != null)
        {
            client.Update();
        }
   
    }

}
