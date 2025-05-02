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
        if(mimic != null) mimic.GetComponent<Animal>().onAnimalDestroy += OnMimicDestroyed;
    }

    void Update()
    {
        rect.anchoredPosition = minimap.WorldToMinimap(mimic.transform.position);
    }


    void OnDisable()
    {
        if(mimic != null) mimic.GetComponent<Animal>().onAnimalDestroy -= OnMimicDestroyed;
    }

    public void SetMimic(ref GameObject gameObject)
    {
        if(mimic != null) mimic.GetComponent<Animal>().onAnimalDestroy -= OnMimicDestroyed;
        mimic = gameObject;
        Animal animal = mimic.GetComponent<Animal>();
        image.sprite = animal.BlipIcon;
        animal.onAnimalDestroy += OnMimicDestroyed;
    }

    void OnMimicDestroyed() {
        Destroy(gameObject);
    }
}
