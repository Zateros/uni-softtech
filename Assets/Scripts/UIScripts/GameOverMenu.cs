using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Pops up menu on GameManager event onGameOver and handles button interactions.
/// </summary>
public class GameOverMenu : MonoBehaviour
{
    public GameObject GameOverPanel;
    public GameObject GameOverBgPanel;
    
    void Start()
    {
        GameManager.Instance.onGameOver += OnGameOver;
    }

    /// <summary>
    /// Handles onGameOver event, pops up panels, stops game
    /// </summary>
    private void OnGameOver()
    {
        GameOverBgPanel.SetActive(true);
        GameOverPanel.SetActive(true);
        GameManager.Instance.IsGameRunnning = false;
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Restarts the game.
    /// </summary>
    public void OnRetryBtnClick()
    {
        GameManager.Instance.IsGameRunnning = true;
        SceneManager.LoadSceneAsync(1);
    }

    /// <summary>
    /// Quits the game, loads main menu.
    /// </summary>
    public void OnMainMenuBtnClick()
    {
        SceneManager.LoadSceneAsync(0);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void OnQuitBtnClick()
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        GameManager.Instance.onGameOver -= OnGameOver;
    }
}
