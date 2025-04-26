using System.Collections;
using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(CountDown(15));
    }

    IEnumerator CountDown(float value)
    {
        float normalizedTime = 0;
        while (normalizedTime <= 1)
        {
            normalizedTime += Time.deltaTime / value;
            yield return null;
        }
        Destroy(gameObject);
    }
}
