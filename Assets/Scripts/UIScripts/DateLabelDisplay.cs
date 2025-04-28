using TMPro;
using UnityEngine;

public class DateLabelDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;
    void Update()
    {
        text.text = GameManager.Instance.Date.AddDays(GameManager.Instance.DaysPassed).ToShortDateString();
    }
}