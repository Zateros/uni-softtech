using UnityEngine;
using TMPro;

public class Notifier : MonoBehaviour
{
    public static Notifier Instance;

    public GameObject NotificationText;

    private int turistCount;

    public bool NotifiedTuristCount;

    private void Awake()
    {
        Instance = this;

        NotifiedTuristCount = false;
    }

    void Update()
    {
        if (turistCount < GameManager.Instance.MinTuristCount + 1)
        {
            if (!NotifiedTuristCount)
            {
                Notify($"Turist count is low ({turistCount})!\nMin turist count: {GameManager.Instance.MinTuristCount}");
                NotifiedTuristCount = true;
            }
        }
        else
            NotifiedTuristCount = false;
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