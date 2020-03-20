using System.IO;
using NetworkProtocal;
using UnityEngine;
using ProtoBuf;

public class KCPTest : MonoBehaviour
{
    private MobaClient m_client;

    // Start is called before the first frame update
    void Start()
    {
        m_client = new MobaClient("127.0.0.1",1001,1);
    }

    void OnGUI()
    {
        if (GUILayout.Button("Send", GUILayout.Width(200), GUILayout.Height(60)))
        {
            EnterRoomRequest request = new EnterRoomRequest();
            request.id = 1;

            byte[] buffer = null;

            using (MemoryStream m = new MemoryStream()) {
                Serializer.Serialize<EnterRoomRequest>(m, request);
            
                m.Position = 0;
                int length = (int)m.Length;
                buffer = new byte[length];
                m.Read(buffer, 0, length);
            }
            m_client.Send(1, buffer, buffer.Length);
            Debug.Log("send data");
        }
    }


    void Update()
    {
        if (m_client != null)
        {
            m_client.Update();
        }
   
    }

}
