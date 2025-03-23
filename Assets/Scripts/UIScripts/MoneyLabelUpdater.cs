using UnityEngine;
using TMPro;

public class MoneyLabelUpdater : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI MoneyLabel;

    // Update is called once per frame
    void Update()
    {
        MoneyLabel.text = "$ " + GameManager.Instance.Money.ToString();
    }
}
