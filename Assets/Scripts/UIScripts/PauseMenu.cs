using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] public GameObject PausePanel;
    [SerializeField] public GameObject PauseBgPanel;

    public void OnSettingBtnClick()
    {
        PauseBgPanel.SetActive(true);
        PausePanel.SetActive(true);
    }

    public void OnReturnBtnClick()
    {
        PauseBgPanel.SetActive(false);
        PausePanel.SetActive(false);
    }

    public void OnQuitBtnClick()
    {
        //SceneManager.LoadScene(0);
    }
}
