using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Controls Sell mode toggle.
/// </summary>
public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
{
    [Header("Slider setup")]
    [SerializeField, Range(0, 1f)]
    protected float sliderValue;
    [Header("Animation")]
    [SerializeField, Range(0, 1f)] private float animationDuration = 0.25f;
    [SerializeField]
    private AnimationCurve _slideEase =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    public GameObject AnimalPanel;
    public GameObject PlantPanel;
    public GameObject ElsePanel;


    private Slider _slider;
    private Camera _mainCamera;
    private RaycastHit2D _hit2D;
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
        _mainCamera = Camera.main;
        IsToggled = false;
        SetupToggleComponents();
    }

    /// <summary>
    /// If toggle is toggled then sells items that the player clicked on.
    /// </summary>
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (IsToggled && Mouse.current.leftButton.wasPressedThisFrame)
        {
            _hit2D = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
                
            if (_hit2D.collider == null) { return; }
                
            GameObject hitObject = _hit2D.collider.gameObject;
                
            if (hitObject.tag == "Animal" || hitObject.tag == "Plant" || hitObject.tag == "Vehicle" || hitObject.tag == "Road")
            {
                GameManager.Instance.Sell(hitObject);
                Destroy(hitObject);
            }
        }
    }

    /// <summary>
    /// Sets up toggle for use.
    /// </summary>
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

    /// <summary>
    /// Deactivates shop panels, swithces toggle on and off.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        IsToggled = !IsToggled;

        if (IsToggled)
        {
            AnimalPanel.SetActive(false);
            PlantPanel.SetActive(false);
            ElsePanel.SetActive(false);
        }

        if (_animateSliderCoroutine != null)
            StopCoroutine(_animateSliderCoroutine);

        _animateSliderCoroutine = StartCoroutine(AnimateSlider());
    }

    /// <summary>
    /// Animate the toggle.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AnimateSlider()
    {
        float startValue = _slider.value;
        float endValue = IsToggled ? 1 : 0;

        float time = 0;
        if (animationDuration > 0)
        {
            while (time < animationDuration)
            {
                time += 0.003f;

                float lerpFactor = _slideEase.Evaluate(time / animationDuration);
                _slider.value = sliderValue = Mathf.Lerp(startValue, endValue, lerpFactor);

                transitionEffect?.Invoke();

                yield return null;
            }
        }

        _slider.value = endValue;
    }
}