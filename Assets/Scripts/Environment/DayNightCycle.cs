using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField]
    private float dayLength = 10f; // Seconds
    [SerializeField]
    private float nightLength = 10f; // Seconds
    [SerializeField]
    private float transitionSpeed = .1f;
    [SerializeField]
    private float minLightIntensity = 0.2f;
    [SerializeField]
    private Light2D light2d;

    public Color dayColor = new Color(0.56f, 0.83f, 1f);

    public Color nightColor = new Color(0.28f, 0.29f, 0.47f);

    private float currentTime = 1f; // [0..1] - 1f day, 0f night

    void Start()
    {
        StartCoroutine(WaitFor(dayLength));
    }

    void FixedUpdate() {
        light2d.intensity = Mathf.Lerp(light2d.intensity, Math.Clamp(currentTime, minLightIntensity, 1f), transitionSpeed);
        if(currentTime == 1f) { Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, dayColor, transitionSpeed); }
        else { Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, nightColor, transitionSpeed); }
    }

    IEnumerator WaitFor(float time) {
        yield return new WaitForSeconds(time);
        CycleTime();
    }

    private void CycleTime() {
        currentTime = currentTime == 0f ? 1f : 0f;
        GameManager.Instance.IsNight = currentTime == 0f;
        StartCoroutine(WaitFor(currentTime == 0f ? nightLength : dayLength));
    }
}
