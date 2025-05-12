using System.Collections;
using UnityEngine;


/// <summary>
/// Destroys current object after a given time.
/// </summary>

public class DestroyOverTime : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(CountDown(15));
    }

    /// <summary>
    /// Waits then destroys object.
    /// </summary>
    /// <param name="value">Time to wait before destruction</param>
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
