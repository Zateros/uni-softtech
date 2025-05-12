using UnityEngine;
using TMPro;

/// <summary>
/// Sends in-game notifications to the player.
/// </summary>
public class Notifier : MonoBehaviour
{
    public GameObject ScrollView;
    public GameObject NotifHolder;
    public static Notifier Instance;
    public GameObject NotificationText;

    public bool notifiedMonthsReset;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Deavtivates notification holder panel if there are no notifications.
    /// </summary>
    void Update()
    {
        if(NotifHolder.transform.childCount == 0)
            ScrollView.SetActive(false);
    }

    /// <summary>
    /// Activates notification panel if it wasnt active before.
    /// Creates and sends new notification to player.
    /// </summary>
    /// <param name="message">Notification message</param>
    public void Notify(string message)
    {
        if(!ScrollView.activeSelf)
            ScrollView.SetActive(true);
        
        GameObject newNotification = Instantiate(NotificationText);
        newNotification.GetComponent<RectTransform>().SetParent(transform);
        newNotification.GetComponent<RectTransform>().localScale = Vector3.one;

        GameObject notificationText = newNotification.transform.GetChild(0).gameObject;
        notificationText.GetComponent<RectTransform>().localScale = Vector3.one;
        notificationText.GetComponent<TextMeshProUGUI>().text = message;
    }
}
