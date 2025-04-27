using UnityEngine;

public class BackBtn : MonoBehaviour
{
    [SerializeField] public GameObject CurrentPanel;

    public void OnBackBtnClick()
    {
        CurrentPanel.SetActive(false);
    }
}
