using UnityEngine;
using UnityEngine.UI;

public class Blip : MonoBehaviour
{
    private Image image;
    public GameObject mimic;
    public Minimap minimap;
    void OnEnable()
    {
        image = GetComponent<Image>();
        image.sprite = mimic.GetComponent<Animal>().BlipIcon;
    }

    void Update()
    {
        transform.position = minimap.WorldToMinimap(mimic.transform.position);
    }
}
