using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles settings.
/// </summary>
public class SettingsScripts : MonoBehaviour
{
    public Toggle FullScreenToggle;
    public Toggle VSyncToggle;

    /// <summary>
    /// Changes the checkboxes based on current screen and vsync settings.
    /// </summary>
    void Start()
    {
        FullScreenToggle.isOn = Screen.fullScreen;

        if(QualitySettings.vSyncCount == 0)
        {
            VSyncToggle.isOn = false;
        }
        else
        {
            VSyncToggle.isOn= true;
        }
    }

    /// <summary>
    /// Changes screen, vsync setting based on the checkboxes.
    /// </summary>
    public void ApplyGraphics()
    {
        Screen.fullScreen = FullScreenToggle.isOn;

        if (VSyncToggle.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }
}
