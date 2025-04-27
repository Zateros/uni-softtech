using UnityEngine;

public class BackBtn : MonoBehaviour
{
    [SerializeField] public GameObject PreviousPanel;
    [SerializeField] public GameObject CurrentPanel;

    public void OnBackBtnClick()
    {
        CurrentPanel.SetActive(false);
        if(PreviousPanel != null)
            PreviousPanel.SetActive(true);
    }
}
