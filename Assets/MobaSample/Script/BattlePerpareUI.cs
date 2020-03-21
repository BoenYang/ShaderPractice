using NetworkProtocal;
using UnityEngine;

public class BattlePerpareUI : MonoBehaviour
{
    public PlayerShow[] PlayerShow;

    public void Show()
    {
        for (int i = 0; i < PlayerShow.Length; i++)
        {
            if (i < MobaData.RoomPlayerInfos.Count) {
                PlayerInfo playerInfo = MobaData.RoomPlayerInfos[i];
                PlayerShow[i].SetPlayerInfo(playerInfo);
                PlayerShow[i].SetReady(playerInfo.ready);
            }
            else
            {
                PlayerShow[i].SetWaiting();
            }
        }
    }

    public void NewPlayerEnter(PlayerInfo playerInfo)
    {

    }

    public void updatePlayerInfo(PlayerInfo info)
    {
        for (int i = 0; i < PlayerShow.Length; i++)
        {
            if (PlayerShow[i].PlayerInfo.id == info.id)
            {
                PlayerShow[i].SetReady(info.ready);
            }
        }
    }

    public void OnReadyClick()
    {
        ReadyRequest request = new ReadyRequest();
        PlayerInfo myInfo = MobaData.GetMyPlayerInfo();
        request.ready = !myInfo.ready;
        MobaClient.Instance.Send(GameCmd.ReadyRequest,request);
    }
}
