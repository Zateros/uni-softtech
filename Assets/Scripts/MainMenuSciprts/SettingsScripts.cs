using UnityEngine;
using UnityEngine.UI;

public class SettingsScripts : MonoBehaviour
{
    [SerializeField] public Toggle FullScreenToggle;
    [SerializeField] public Toggle VSyncToggle;

    public string playerName;

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

    public void GetName(string input)
    {
        playerName = input;
    }
}
