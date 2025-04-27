using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
{
    [Header("Slider setup")]
    [SerializeField, Range(0, 1f)]
    protected float sliderValue;
    [Header("Animation")]
    [SerializeField, Range(0, 1f)] private float animationDuration = 0.25f;
    [SerializeField]
    private AnimationCurve slideEase =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    public GameObject AnimalPanel;
    public GameObject PlantPanel;
    public GameObject ElsePanel;
    public Button AnimalButton;
    public Button PlantButton;
    public Button ElseButton;


    private Slider _slider;
    private Camera mainCamera;
    private RaycastHit2D hit2D;
    private Coroutine _animateSliderCoroutine;
    protected Action transitionEffect;

    public bool IsToggled { get; private set; }

    void OnValidate()
    {
        SetupToggleComponents();
        _slider.value = sliderValue;
    }

    void Awake()
    {
        mainCamera = Camera.main;
        IsToggled = false;
        SetupToggleComponents();
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (IsToggled && Mouse.current.leftButton.wasPressedThisFrame)
        {
            hit2D = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
                
            if (hit2D.collider == null) { return; }
                
            GameObject hitObject = hit2D.collider.gameObject;
                
            if (hitObject.tag == "Animal" || hitObject.tag == "Plant" || hitObject.tag == "Vehicle" || hitObject.tag == "Road")
            {
                GameManager.Instance.Sell(hitObject);
                Destroy(hitObject);
            }
        }
    }

    private void SetupToggleComponents()
    {
        if (_slider != null)
            return;

        _slider = GetComponent<Slider>();

        if (_slider == null)
        {
            return;
        }

        _slider.interactable = false;
        var sliderColors = _slider.colors;
        sliderColors.disabledColor = Color.white;
        _slider.colors = sliderColors;
        _slider.transition = Selectable.Transition.None;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        IsToggled = !IsToggled;

        if (IsToggled)
        {
            AnimalPanel.SetActive(false);
            PlantPanel.SetActive(false);
            ElsePanel.SetActive(false);
            AnimalButton.enabled = false;
            PlantButton.enabled = false;
            ElseButton.enabled = false;
        }
        else
        {
            AnimalButton.enabled = true;
            PlantButton.enabled = true;
            ElseButton.enabled = true;
        }

        if (_animateSliderCoroutine != null)
            StopCoroutine(_animateSliderCoroutine);

        _animateSliderCoroutine = StartCoroutine(AnimateSlider());
    }


    private IEnumerator AnimateSlider()
    {
        float startValue = _slider.value;
        float endValue = IsToggled ? 1 : 0;

        float time = 0;
        if (animationDuration > 0)
        {
            while (time < animationDuration)
            {
                time += Time.deltaTime;

                float lerpFactor = slideEase.Evaluate(time / animationDuration);
                _slider.value = sliderValue = Mathf.Lerp(startValue, endValue, lerpFactor);

                transitionEffect?.Invoke();

                yield return null;
            }
        }

        _slider.value = endValue;
    }
}