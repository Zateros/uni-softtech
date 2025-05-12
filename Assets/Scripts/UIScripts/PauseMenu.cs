using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Handles PauseBtnClick, opens pause menu.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject PauseBgPanel;

    /// <summary>
    /// Opens up the pause menu, stops game.
    /// </summary>
    public void OnSettingBtnClick()
    {
        PauseBgPanel.SetActive(true);
        PausePanel.SetActive(true);
        GameManager.Instance.IsGameRunnning = false;
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Closes pause menu, resumes the game.
    /// </summary>
    public void OnReturnBtnClick()
    {
        Time.timeScale = GameManager.Instance.PrevSpeed;
        PauseBgPanel.SetActive(false);
        PausePanel.SetActive(false);
        GameManager.Instance.IsGameRunnning = true;
    }

    /// <summary>
    /// Quits the game, loads main menu.
    /// </summary>
    public void OnQuitBtnClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
    }
}
