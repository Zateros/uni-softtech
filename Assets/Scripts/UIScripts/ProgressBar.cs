using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] public Image ProgressImage;
    [SerializeField] public float DefaultSpeed = 1f;
    [SerializeField] public TextMeshProUGUI Percentage;

    private Coroutine AnimationCoroutine;

    void Start()
    {
        InvokeRepeating("SetProgress", 1f, 1f);
    }

    public void SetProgress()
    {
        float progress = GameManager.Instance.CalculateSatisfaction() / (float)100;

        if (progress != ProgressImage.fillAmount)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }

            AnimationCoroutine = StartCoroutine(AnimateProgress(progress));

            Percentage.text = (progress * 100).ToString() + " %";
        }
    }

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
