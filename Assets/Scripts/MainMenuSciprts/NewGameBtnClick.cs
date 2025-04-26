using UnityEngine;

public class NewGameBtnClick : MonoBehaviour
{
    [SerializeField]
    public GameObject SetProfilePanel;

    public void OnBtnClick()
    {
        SetProfilePanel.SetActive(true);
    }
}
