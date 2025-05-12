using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Pops up menu on GameManager event onGameWon and handles button interactions.
/// </summary>
public class GameWonMenu : MonoBehaviour
{
    public GameObject GameWonPanel;
    public GameObject GameWonBgPanel;

    void Start()
    {
        GameManager.Instance.onGameWon += OnGameWon;
    }

    /// <summary>
    /// Handles onGameWon event, pops up panels, stops game
    /// </summary>
    private void OnGameWon()
    {
        GameWonBgPanel.SetActive(true);
        GameWonPanel.SetActive(true);
        GameManager.Instance.IsGameRunnning = false;
        Time.timeScale = 0f;
    }


    /// <summary>
    /// Resumes game.
    /// </summary>
    public void OnReturnBtnClick()
    {
        Time.timeScale = GameManager.Instance.PrevSpeed;
        GameManager.Instance.IsGameRunnning = true;
        GameWonBgPanel.SetActive(false);
        GameWonPanel.SetActive(false);
    }

    /// <summary>
    /// Quits the game, loads main menu.
    /// </summary>
    public void OnMainMenuBtnClick()
    {
        SceneManager.LoadSceneAsync(0);
    }


    /// <summary>
    /// Quits the game.
    /// </summary>
    public void OnQuitBtnClick()
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        GameManager.Instance.onGameWon -= OnGameWon;
    }
}
