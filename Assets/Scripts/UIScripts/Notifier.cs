using UnityEngine;
using TMPro;
using UnityEngine.Playables;

public class Notifier : MonoBehaviour
{
    public GameObject NotificationText;


    void Update()
    {
        if(GameManager.Instance.Money <= 1400)
        {
            enabled = false;
            Notify($"Money low: {GameManager.Instance.Money}");
            Notify("Second notif to see if works");
            Notify("Third notif to see if works");
        }
    }

    public void Notify(string message)
    {
        GameObject newNotification = Instantiate(NotificationText);
        newNotification.GetComponent<RectTransform>().SetParent(transform);
        newNotification.GetComponent<RectTransform>().localScale = Vector3.one;

        GameObject notificationText = newNotification.transform.GetChild(0).gameObject;
        notificationText.GetComponent<RectTransform>().localScale = Vector3.one;
        notificationText.GetComponent<TextMeshProUGUI>().text = message;
    }
}
