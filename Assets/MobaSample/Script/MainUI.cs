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

    void Awake()
    {
        MobaData.MyId = userId;
    }

    public void OnEnterRoomClick()
    {
        EnterRoomRequest request = new EnterRoomRequest();
        request.id = userId;
        request.name = userName;
        MobaClient.Instance.Send(GameCmd.EnterRoomRequest,request);
    }

}
