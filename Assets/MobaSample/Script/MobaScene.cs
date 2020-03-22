using System;
using System.Collections;
using System.Collections.Generic;
using NetworkProtocal;
using UnityEngine;

public class MobaScene : MonoBehaviour
{

    public GameObject HeroPrefab;

    private List<MobaPlayer> m_PlayerList;

    private MobaPlayer myPlayer;

    private bool m_GameStart;

    private GameObject m_MyServerPos;

    private bool m_GameStarted = false;

    private float m_LastSyncTime = 0;

    private int SyncIntervalMs = 100;

    void Start()
    {
        m_PlayerList = new List<MobaPlayer>();

        if (MobaData.PlayerGameInfos != null)
        {

            for (int i = 0; i < MobaData.PlayerGameInfos.Count; i++)
            {
                PlayerGameInfo info = MobaData.PlayerGameInfos[i];
                Vector3 pos = new Vector3(info.PosX/100f,info.PosY/100f,info.PosZ/100f);
                GameObject go = Instantiate(HeroPrefab, pos,Quaternion.identity);
                MobaPlayer player = go.AddComponent<MobaPlayer>();
                if (info.PlayerId == MobaData.MyId)
                {
                    go.AddComponent<MobaPlayerInput>();
                    myPlayer = player;
                }

                m_MyServerPos = Instantiate(HeroPrefab, pos, Quaternion.identity);
            }
            MobaClient.Instance.AddCmdListener(GameCmd.SyncBroadcast,this.OnSync);
            MobaClient.Instance.AddCmdListener(GameCmd.PlayerInitedBroadcast, this.OnAllPlayerInited);
            MobaClient.Instance.Send(GameCmd.PlayerInitedRequest,new PlayerInitedReqeust());
        }
    }

    void Update() {
        if (myPlayer) {
            if (m_GameStarted)
            {
                float currentTime = Time.time;
                float lastSyncInterval = currentTime - m_LastSyncTime;
                if (lastSyncInterval * 1000 > SyncIntervalMs)
                {
                    myPlayer.SyncPos();
                    m_LastSyncTime = Time.time;
                }
            }
        }
    }

    void OnAllPlayerInited(byte[] data, int size)
    {
        m_GameStarted = true;
    }

    void OnSync(byte[] data, int size)
    {
        PlayerGameInfo gameInfo = PBUtils.PBDeserialize<PlayerGameInfo>(data);

        Debug.LogFormat("[MobaScene] x = {0} y = {1} z = {2}", gameInfo.PosX/100f, gameInfo.PosY/100f, gameInfo.PosZ/100f);

        if(m_MyServerPos)
            m_MyServerPos.transform.position = new Vector3(gameInfo.PosX/100f,gameInfo.PosY/100f,gameInfo.PosZ/100f);
    }
}
