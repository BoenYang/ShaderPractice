using System.Collections;
using System.Collections.Generic;
using System.IO;
using NetworkProtocal;
using ProtoBuf;
using UnityEngine;

public class MainUI : MonoBehaviour
{

    public string userName;

    public uint userId;

    void Start()
    {
        MobaData.MyId = userId;
        MobaClient client = new MobaClient("127.0.0.1",1001, userId);
    }

    public void OnEnterRoomClick()
    {
        EnterRoomRequest request = new EnterRoomRequest();
        request.id = userId;
        request.name = userName;
        MobaClient.Instance.Send(GameCmd.EnterRoomRequest,request);
    }

    void Update()
    {
        if (MobaClient.Instance != null)
        {
            MobaClient.Instance.Update();
        }
    }
}
