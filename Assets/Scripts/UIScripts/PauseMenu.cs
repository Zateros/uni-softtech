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
        GameManager.Instance.IsGameRunnning = false;
        Time.timeScale = 0f;
    }

    public void OnReturnBtnClick()
    {
        Time.timeScale = GameManager.Instance.PrevSpeed;
        PauseBgPanel.SetActive(false);
        PausePanel.SetActive(false);
        GameManager.Instance.IsGameRunnning = true;
    }

    public void OnQuitBtnClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
    }
}
