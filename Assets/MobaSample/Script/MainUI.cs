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
        MobaClient client = new MobaClient("127.0.0.1",1001, userId);
    }

    public void OnEnterRoomClick()
    {
        EnterRoomRequest request = new EnterRoomRequest();
        request.playerInfo = new PlayerInfo();
        request.playerInfo.id = userId;
        request.playerInfo.name = userName;
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
