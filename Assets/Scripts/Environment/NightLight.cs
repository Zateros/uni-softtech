using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NightLight : MonoBehaviour
{
    private Light2D light;
    public bool isRoad = false;
    private Animal animal;

    void OnEnable()
    {
        light = GetComponent<Light2D>();
        if (!isRoad) animal = GetComponentInParent<Animal>();
        DayNightCycle.onNight += EnableLight;
        DayNightCycle.onDay += DisableLight;
    }

    void OnDisable()
    {
        DayNightCycle.onNight -= EnableLight;
        DayNightCycle.onDay -= DisableLight;
    }

    void EnableLight() {
        if (!isRoad && !animal.HasChip) return;
        light.enabled = true;
        Debug.Log(light.enabled);
    }
    void DisableLight() {
        if (!isRoad && !animal.HasChip) return;
        light.enabled = false;
        Debug.Log(light.enabled);
    }
}
