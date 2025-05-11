using UnityEngine;
using TMPro;
using System;

public class Notifier : MonoBehaviour
{
    [SerializeField] public GameObject ScrollView;
    [SerializeField] public GameObject NotifHolder;
    public static Notifier Instance;
    public GameObject NotificationText;

    public bool NotifiedMonthsReset;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if(NotifHolder.transform.childCount == 0)
            ScrollView.SetActive(false);
    }

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
