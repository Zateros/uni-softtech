using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Allows player to choose difficulty.
/// </summary>
public class DifficultyBtns : MonoBehaviour
{
    public Button EasyBtn;
    public Button MediumBtn;
    public Button HardBtn;

    public static int difficulty = 0;

    void Awake()
    {
        EasyBtnClick();
    }

    /// <summary>
    /// Sets difficulty to easy, disables easy button.
    /// </summary>
    public void EasyBtnClick()
    {
        EasyBtn.enabled = false;
        MediumBtn.enabled = true;
        HardBtn.enabled = true;

        EasyBtn.GetComponent<Image>().color = Color.gray;
        MediumBtn.GetComponent<Image>().color = Color.white;
        HardBtn.GetComponent<Image>().color = Color.white;

        difficulty = 0;
    }


    /// <summary>
    /// Sets difficulty to medium, disables medium button.
    /// </summary>
    public void MediumBtnClick()
    {
        EasyBtn.enabled = true;
        MediumBtn.enabled = false;
        HardBtn.enabled = true;

        EasyBtn.GetComponent<Image>().color = Color.white;
        MediumBtn.GetComponent<Image>().color = Color.gray;
        HardBtn.GetComponent<Image>().color = Color.white;

        difficulty = 1;
    }

    /// <summary>
    /// Sets difficulty to hard, disables hard button.
    /// </summary>
    public void HardBtnClick()
    {
        EasyBtn.enabled = true;
        MediumBtn.enabled = true;
        HardBtn.enabled = false;

        EasyBtn.GetComponent<Image>().color = Color.white;
        MediumBtn.GetComponent<Image>().color = Color.white;
        HardBtn.GetComponent<Image>().color = Color.gray;

        difficulty = 2;
    }
}
