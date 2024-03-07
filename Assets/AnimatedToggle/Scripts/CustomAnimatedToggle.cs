using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomAnimatedToggle : Selectable, IPointerClickHandler, ISubmitHandler
{
    [SerializeField] private bool _isOn;

    [Header("Toggle Parameters")]
    [SerializeField] private Color _backGroundDisabledColor;
    [SerializeField] private Color _backGroundEnabledColor;
    [SerializeField] private Transform _circleDisabledTransform;
    [SerializeField] private Transform _circleEnabledTransform;

    [Header("Animation Parameters")]
    [SerializeField] private float _animationSpeed = 1f;
    [SerializeField] private float _xMoveScale;
    [SerializeField] private Ease _colorChangingEase;
    [SerializeField] private Ease _dinamicBackgroundScaleEase;
    [SerializeField] private Ease _circleMoveEase;
    [SerializeField] private Ease _circleScaleXEase;
    
    [Header("Toggle Elements")]
    [SerializeField] private Image _backGroundStaticImage;
    [SerializeField] private RectTransform _backGroundDinamicTransform;
    [SerializeField] private RectTransform _circle;

    //if you like this
    public UnityEvent<bool> OnValueChangedUEvent;

    public event Action<bool> OnValueChangedAction;

    public bool IsOn => _isOn;

    //Init it here if you need
    //protected override void Awake()
    //{
    //    base.Awake();

    //    SetValue(_isOn);
    //}

    public void OnPointerClick(PointerEventData eventData)
    {
        Interact();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        Interact();
    }

    public void SetValue(bool value)
    {
        _isOn = value;

        UpdateVisualState();
    }

    private void Interact()
    {
        _isOn = !_isOn;

        OnValueChangedUEvent?.Invoke(_isOn);
        OnValueChangedAction?.Invoke(_isOn);

        PlayAnimation();
    }

    private void PlayAnimation()
    {
        Color targetBackroungColor = GetBackgroungColor();
        Vector3 targetBackroundScale = GetBackgroundScale();
        Vector3 targetCirclePosition = GetCircleTargetPosition();

        //change color
        _backGroundStaticImage.DOColor(targetBackroungColor, _animationSpeed).SetEase(_colorChangingEase);
        //change scale
        _backGroundDinamicTransform.DOScale(targetBackroundScale, _animationSpeed).SetEase(_dinamicBackgroundScaleEase);
        //change position
        _circle.DOMove(targetCirclePosition, _animationSpeed).SetEase(_circleMoveEase);

        Sequence scaleSequence = DOTween.Sequence();

        //bouncy move fx
        //on time scale -> | change scale | wait | wait | change scale back |
        scaleSequence
            .Append(_circle.DOScaleX(1 + _xMoveScale, _animationSpeed / 4f))
            .Append(_circle.DOScaleX(1, _animationSpeed / 4f))
            .SetDelay(_animationSpeed / 2f)
            .SetEase(_circleScaleXEase)
            .OnComplete(() => scaleSequence.Kill());
    }

    /// <summary>
    /// If you change _isOn - you can change the visual state from the component context menu.
    /// </summary>
    [ContextMenu("Update Visual State")]
    private void UpdateVisualState()
    {
        _backGroundStaticImage.color = GetBackgroungColor();
        _backGroundDinamicTransform.localScale = GetBackgroundScale();
        _circle.position = GetCircleTargetPosition();
    }

    private Vector3 GetCircleTargetPosition() => _isOn ? _circleEnabledTransform.position : _circleDisabledTransform.position;
    private Vector3 GetBackgroundScale() => _isOn ? Vector3.zero : Vector3.one;
    private Color GetBackgroungColor() => _isOn ? _backGroundEnabledColor : _backGroundDisabledColor;
}
