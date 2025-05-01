using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_GameScene_CardBase<T> : UI_Base where T : CardBase
{
    #region Enum to Bind
    protected enum GameObjects
    {
        CardButton,
    }

    protected enum Buttons
    {
        CardButton,
    }

    protected enum Images
    {
        CardButton,
        CardImage,
    }
    #endregion

    public T Card { get; protected set; }

    protected CardManager _cardManager { get { return Managers.CardManager; } }

    public RectTransform RectTransform { get; protected set; }
    public RectTransform ImageRectTransform { get; protected set; }

    // Scriptable Objects
    private IdleTiltSetting _tiltSetting;
    private HoverSetting _hoverSetting;

    ECardState _cardUIState = ECardState.None;
    public ECardState CardUIState
    {
        get { return _cardUIState; }
        protected set
        {
            if (_cardUIState == value) return;

            ECardState lastState = _cardUIState;
            _cardUIState = value;

            PlayAnimation(value, lastState);
        }
    }

    #region Init & SetInfo
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        RectTransform = GetComponent<RectTransform>();

        _tiltSetting = Managers.Resource.Load<IdleTiltSetting>("IdleTiltSetting");
        _hoverSetting = Managers.Resource.Load<HoverSetting>("HoverSetting");

        #region Bind
        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));
        #endregion

        #region Event Bind
        GetButton((int)Buttons.CardButton).gameObject.BindEvent
            (
            (UIEvent.PointerEnter,  OnPointerEnter),
            (UIEvent.PointerExit,   OnPointerExit),
            (UIEvent.PointerDown,   OnPointerDown),
            (UIEvent.PointerUp,     OnPointerUp),
            (UIEvent.Click,         OnClick),
            (UIEvent.BeginDrag,     OnBeginDrag),
            (UIEvent.Drag,          OnDrag),
            (UIEvent.EndDrag,       OnEndDrag)
            );
        #endregion

        return true;
    }

    public virtual void SetInfo(T card)
    {
        transform.localScale = Vector3.one;

        Card = card;

        // Setting UI Image
        GetImage((int)Images.CardImage).sprite = Managers.Resource.Load<Sprite>(card.CardData.FrontSpriteName);

        // SetActive -> 1프레임 뒤에 Position Setting // GridLayGroup 초기화 이슈,
        gameObject.SetActive(true);
        ImageRectTransform = GetObject((int)GameObjects.CardButton).GetComponent<RectTransform>();
        StartCoroutine(CaptureOriginalNextFrame());

        // Init State
        CardUIState = card.CardState;
    }
    protected virtual void OnStateChanged(ECardState state)
    {
        CardUIState = state;
    }

    IEnumerator CaptureOriginalNextFrame()
    {
        yield return new WaitForEndOfFrame();
        Card.OriginalPosition = RectTransform.position;
    }
    #endregion

    public void UpdatePositionFromCard()
    {
        RectTransform.position = Card.OriginalPosition;
    }

    protected virtual bool TrySwap(PointerEventData evt, bool isBoth)
    {
        if (evt.pointerEnter == null)
            return false;

        return true;
    }

    #region Animation

    void PlayAnimation(ECardState currentUiState, ECardState lastUiState)
    {
        if (_coCardTilt != null) StopCoroutine(_coCardTilt); _coCardTilt = null;
        if (lastUiState == ECardState.Hover) PlayDeHoverAnim();

        switch (currentUiState)
        {
            case ECardState.Idle:
                PlayIdleAnim();
                break;
            case ECardState.Hover:
                PlayHoverAnim();
                break;
            case ECardState.Select:
                break;
            case ECardState.Moving:
                break;
        }
    }

    #region Idle Animation
    protected virtual void PlayIdleAnim()
    {
        startOffset = Random.Range(0f, Mathf.PI * 2f); // Animation 시작 지점 조정

        _coCardTilt = StartCoroutine(CoCardTilt());
    }

    Coroutine _coCardTilt;
    IEnumerator CoCardTilt()
    {
        while (Card.CardState == ECardState.Idle)
        {
            CardTilt();
            yield return null;
        }
    }

    private float startOffset; // 시작 시점을 달리하여, 각 카드가 다른 Motion일 수 있도록 만들기 위함
    private void CardTilt()
    {
        // 사인, 코사인 계산
        float time = Time.time + startOffset;
        float sinValue = Mathf.Sin(time);
        float cosValue = Mathf.Cos(time);

        // 현재 Euler 각도 읽기
        Vector3 euler = ImageRectTransform.eulerAngles;

        // X, Y 축 각각 LerpAngle 보간
        float newX = Mathf.LerpAngle(euler.x, sinValue * _tiltSetting.maxAngle, _tiltSetting.lerpSpeed * Time.deltaTime);
        float newY = Mathf.LerpAngle(euler.y, cosValue * _tiltSetting.maxAngle, _tiltSetting.lerpSpeed * Time.deltaTime);

        // Apply
        ImageRectTransform.eulerAngles = new Vector3(newX, newY, 0f);
    }
    #endregion

    #region Hover Animation
    protected virtual void PlayHoverAnim()
    {
        if (_hoverSetting.IsApplyScaleAnimation)
            RectTransform.DOScale(_hoverSetting.scaleOnHover, _hoverSetting.scaleTransition).SetEase(Ease.OutBack);

        DOTween.Kill(2, true);
        RectTransform.DOPunchRotation(Vector3.forward * _hoverSetting.hoverPunchAngle, _hoverSetting.hoverTransition, 20, 1).SetId(2);

    }

    private void PlayDeHoverAnim(float deltaFromOriginalSize = 1) // 1 -> OriginSize
    {
        RectTransform.DOScale(1, _hoverSetting.scaleTransition).SetEase(Ease.OutBack);
    }
    #endregion

    #endregion

    #region Event Handle
    protected virtual void OnPointerEnter(PointerEventData evt)
    {
        Card.CardState = ECardState.Hover;

        Debug.Log("On PointerEnter");
    }

    protected virtual void OnPointerExit(PointerEventData evt)
    {
        if (HasMoved()) return;

        Card.CardState = ECardState.Idle;

        Debug.Log("On PointerExit");
    }

    protected Vector3 _dragOffset;
    protected virtual void OnPointerDown(PointerEventData evt)
    {
        _dragOffset = RectTransform.position - (Vector3)evt.position;
    }

    protected virtual void OnPointerUp(PointerEventData evt)
    {
        /*if (scaleAnimations)
            transform.DOScale(longPress ? scaleOnHover : scaleOnSelect, scaleTransition).SetEase(scaleEase);

        visualShadow.localPosition = shadowDistance;*/

        Card.CardState = ECardState.Idle;
        RectTransform.position = Card.OriginalPosition;
        GetImage((int)Images.CardButton).raycastTarget = true;

        Debug.Log("On End Drag");
    }

    protected virtual void OnClick(PointerEventData evt)
    {
        if (HasMoved()) return;
        Card.CardState = ECardState.Select;

        Debug.Log("On Click");
    }
    
    protected virtual void OnBeginDrag(PointerEventData evt)
    {
        
    }

    protected virtual void OnDrag(PointerEventData evt)
    {
        Debug.Log($"On Drag");

        Card.CardState = ECardState.Moving;

        GetImage((int)Images.CardButton).raycastTarget = false;

        RectTransform.position = (Vector3)evt.position + _dragOffset;
    }

    protected virtual void OnEndDrag(PointerEventData evt)
    {
        
    }
    #endregion

    #region Helper
    protected bool HasMoved()
    {
        return Card.OriginalPosition != RectTransform.position;
    }
    #endregion
}
