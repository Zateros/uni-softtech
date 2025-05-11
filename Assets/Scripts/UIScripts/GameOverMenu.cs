using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject GameOverPanel;
    public GameObject GameOverBgPanel;
    
    void Start()
    {
        GameManager.Instance.onGameOver += OnGameOver;
    }

    private void OnGameOver()
    {
        GameOverBgPanel.SetActive(true);
        GameOverPanel.SetActive(true);
        GameManager.Instance.IsGameRunnning = false;
        Time.timeScale = 0f;
    }

    public void OnRetryBtnClick()
    {
        GameManager.Instance.IsGameRunnning = true;
        SceneManager.LoadSceneAsync(1);
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
        GameManager.Instance.onGameOver -= OnGameOver;
    }
}
