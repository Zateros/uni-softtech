using UnityEngine;
using UnityEngine.UI;

public class Blip : MonoBehaviour
{
    private Image image;
    public GameObject mimic;
    public Minimap minimap;
    private RectTransform rect;
    void OnEnable()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        rect.anchoredPosition = minimap.WorldToMinimap(mimic.transform.position);
    }

    public void SetMimic(ref GameObject gameObject)
    {
        mimic = gameObject;
        image.sprite = mimic.GetComponent<Animal>().BlipIcon;
    }
}
