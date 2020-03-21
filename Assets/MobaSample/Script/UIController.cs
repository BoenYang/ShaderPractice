
using UnityEngine;

public class UIController : MonoBehaviour
{
    public MainUI mainUI;

    public BattlePerpareUI battlePerpareUI;

    public static UIController Instance;

    void Awake()
    {
        Instance = this;
        battlePerpareUI.gameObject.SetActive(false);
    }

    public void ShowBattlePerpareUI()
    {
        battlePerpareUI.gameObject.SetActive(true);
        battlePerpareUI.Show();
    }

}

