using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWonMenu : MonoBehaviour
{
    public GameObject GameWonPanel;
    public GameObject GameWonBgPanel;

    void Start()
    {
        GameManager.Instance.onGameWon += OnGameWon;
    }

    private void OnGameWon()
    {
        GameWonBgPanel.SetActive(true);
        GameWonPanel.SetActive(true);
        GameManager.Instance.IsGameRunnning = false;
        Time.timeScale = 0f;
    }

    public void OnReturnBtnClick()
    {
        Time.timeScale = GameManager.Instance.PrevSpeed;
        GameManager.Instance.IsGameRunnning = true;
        GameWonBgPanel.SetActive(false);
        GameWonPanel.SetActive(false);
    }

    public void OnMainMenuBtnClick()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void OnQuitBtnClick()
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        GameManager.Instance.onGameWon -= OnGameWon;
    }
}
