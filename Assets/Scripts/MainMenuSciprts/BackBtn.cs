using UnityEngine;

/// <summary>
/// Takes player back to previious panel, sets current panel deactive.
/// </summary>
public class BackBtn : MonoBehaviour
{
    public GameObject PreviousPanel;
    public GameObject CurrentPanel;

    public void OnBackBtnClick()
    {
        CurrentPanel.SetActive(false);
        if(PreviousPanel != null)
            PreviousPanel.SetActive(true);
    }
}
