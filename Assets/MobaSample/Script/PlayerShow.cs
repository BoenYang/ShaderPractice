using NetworkProtocal;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShow : MonoBehaviour
{
    public Image Avatar;

    public Text Id;

    public Text UserName;

    public Text WaitingText;

    public void SetPlayerInfo(PlayerInfo info)
    {
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


}
