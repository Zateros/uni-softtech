using UnityEngine;

/// <summary>
/// Manages main menu interactions.
/// </summary>
public class MenuManager : MonoBehaviour
{
    public GameObject SetProfilePanel;
    public GameObject SettingsPanel;

    /// <summary>
    /// Activates profile panel.
    /// </summary>
    public void OnNewGameBtnClick()
    {
        SetProfilePanel.SetActive(true);
    }

    /// <summary>
    /// Activates settings panel.
    /// </summary>
    public void OnSettingsBtnClick()
    {
        SettingsPanel.SetActive(true);
    }

    /// <summary>
    /// Quits application.
    /// </summary>
    public void OnQuitBtnClick()
    {
        Application.Quit();
    }
}
