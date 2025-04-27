using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class NewGameLoader : MonoBehaviour
{
    [SerializeField] GameObject ConfirmBtn1;
    [SerializeField] GameObject ConfirmBtn2;
    [SerializeField] GameObject CheckBox;
    [SerializeField] GameObject TutorialPanel;
    [SerializeField] TextMeshProUGUI TutorialText;

    public void OnConfirmBtnClick()
    {
        if(CheckBox.GetComponent<Toggle>().isOn)
        {

            TutorialPanel.SetActive(true);
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
