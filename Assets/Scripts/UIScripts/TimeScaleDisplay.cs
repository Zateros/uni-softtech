using TMPro;
using UnityEngine;

public class TimeScaleDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;
    void Update()
    {
        text.text = Time.timeScale.ToString();
    }
}
