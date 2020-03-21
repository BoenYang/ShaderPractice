using NetworkProtocal;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShow : MonoBehaviour
{
    public Image Avatar;

    public Text Id;

    public Text UserName;

    public Text WaitingText;

    public Text ReadyText;

    [System.NonSerialized]
    public PlayerInfo PlayerInfo;

    public void SetPlayerInfo(PlayerInfo info)
    {
        PlayerInfo = info;
        WaitingText.gameObject.SetActive(false);
        Avatar.gameObject.SetActive(true);
        Id.text = info.id + "";
        UserName.text = info.name;
    }

    public void SetWaiting()
    {
        WaitingText.gameObject.SetActive(true);
        Avatar.gameObject.SetActive(false);
    }

    public void SetReady(bool ready)
    {
        ReadyText.gameObject.SetActive(ready);
    }


}
