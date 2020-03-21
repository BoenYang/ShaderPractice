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

    public void SetReady()
    {

    }

    public void OnReadyClick()
    {

    }
}
