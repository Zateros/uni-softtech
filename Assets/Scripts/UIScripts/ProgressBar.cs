using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


/// <summary>
/// Shows turist satisfaction precentage on toolbar.
/// </summary>
public class ProgressBar : MonoBehaviour
{
    [SerializeField] public Image ProgressImage;
    [SerializeField] public float DefaultSpeed = 1f;
    [SerializeField] public TextMeshProUGUI Percentage;

    private Coroutine _animationCoroutine;

    /// <summary>
    /// InvokeRepeating calls SetProgress every second.
    /// </summary>
    void Start()
    {
        InvokeRepeating("SetProgress", 1f, 1f);
    }

    /// <summary>
    /// Calculates precentage based on GameManager CalculateSatisfaction
    /// </summary>
    public void SetProgress()
    {
        float progress = GameManager.Instance.satisfaction / (float)100;

        if (progress != ProgressImage.fillAmount)
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            _animationCoroutine = StartCoroutine(AnimateProgress(progress));

            Percentage.text = (progress * 100).ToString() + " %";
        }
    }

    /// <summary>
    /// Animates the progressbar based on the parameter.
    /// Sets color of progress bar based on the current precentage.
    /// </summary>
    /// <param name="progress">Turist satisfaction converted to 0 - 1 value float </param>
    /// <returns></returns>
    private IEnumerator AnimateProgress(float progress)
    {
        float time = 0;
        float initialProgress = ProgressImage.fillAmount;

        while (time < 1)
        {
            ProgressImage.fillAmount = Mathf.Lerp(initialProgress, progress, time);

            if (ProgressImage.fillAmount <= (GameManager.Instance.MinTuristSatisfaction + 10) / (float)100)
                ProgressImage.color = Color.red;
            else if (ProgressImage.fillAmount <= (GameManager.Instance.MinTuristSatisfaction + 20) / (float)100)
                ProgressImage.color = Color.yellow;
            else
                ProgressImage.color = Color.green;

            time += Time.deltaTime * DefaultSpeed;
            yield return null;
        }

        ProgressImage.fillAmount = progress;
    }
}
