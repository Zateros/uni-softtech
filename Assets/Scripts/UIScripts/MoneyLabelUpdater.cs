using UnityEngine;
using TMPro;

/// <summary>
/// Updates money count on toolbar.
/// </summary>
public class MoneyLabelUpdater : MonoBehaviour
{
    public TextMeshProUGUI MoneyLabel;

    void Update()
    {
        MoneyLabel.text = "$ " + GameManager.Instance.Money.ToString();
    }
}
