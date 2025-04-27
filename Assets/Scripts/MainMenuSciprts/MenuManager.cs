using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] public GameObject SetProfilePanel;
    [SerializeField] public GameObject SettingsPanel;

    public void OnNewGameBtnClick()
    {
        SetProfilePanel.SetActive(true);
    }

    public void OnSettingsBtnClick()
    {
        SettingsPanel.SetActive(true);
    }
}
