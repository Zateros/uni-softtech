using UnityEngine;
using UnityEngine.UI;

public class DifficultyBtns : MonoBehaviour
{
    [SerializeField] Button EasyBtn;
    [SerializeField] Button MediumBtn;
    [SerializeField] Button HardBtn;

    public static int difficulty;

    void Awake()
    {
        EasyBtnClick();
    }

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
