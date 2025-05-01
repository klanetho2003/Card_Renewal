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
        CardShadow,
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
    public RectTransform ShadowRectTransform { get; protected set; }

    // Scriptable Objects
    private IdleTiltSetting _tiltSetting;
    private HoverSetting _hoverSetting;
    private ClickSetting _clickSetting;

    [SerializeField]
    ECardState DebugState_1 = ECardState.None;
    [SerializeField]
    ECardState DebugState_2 = ECardState.None;

    #region Init & SetInfo
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        RectTransform = GetComponent<RectTransform>();

        _tiltSetting = Managers.Resource.Load<IdleTiltSetting>("IdleTiltSetting");
        _hoverSetting = Managers.Resource.Load<HoverSetting>("HoverSetting");
        _clickSetting = Managers.Resource.Load<ClickSetting>("ClickSetting");

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
        ShadowRectTransform = GetObject((int)GameObjects.CardShadow).GetComponent<RectTransform>();
        _originShadowPosition = ShadowRectTransform.localPosition;
        StartCoroutine(CaptureOriginalNextFrame());
    }

    IEnumerator CaptureOriginalNextFrame()
    {
        yield return new WaitForEndOfFrame();
        Card.OriginalPosition = RectTransform.position;

        PlayAnimation(ECardState.Idle, ECardState.None);
    }
    #endregion

    public void UpdatePositionFromCard()
    {
        RectTransform.position = Card.OriginalPosition;
    }

    protected virtual bool TrySwap(PointerEventData evt, bool isBoth)
    {
        if (HasMoved() == false)
            return false;

        if (evt.pointerEnter == null)
            return false;

        return true;
    }

    protected virtual void OnStateChanged(ECardState currentState, ECardState lastState)
    {
        PlayAnimation(currentState, lastState);
        _IsLongPress = false;

        DebugState_1 = currentState;
        DebugState_2 = lastState;
    }

    #region Animation
    void PlayAnimation(ECardState currentUiState, ECardState lastUiState)
    {
        if (_coCardTilt != null) StopCoroutine(_coCardTilt); _coCardTilt = null;

        if (lastUiState == ECardState.Hover)
            PlayDeHoverAnim();

        switch (currentUiState)
        {
            case ECardState.Idle:
                PlayIdleAnim();
                break;
            case ECardState.Hover:
                PlayHoverAnim();
                break;
            case ECardState.PointDown:
                PlayPointerDown();
                break;
            case ECardState.PointUp:
                PlayPointerUp();
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

    protected virtual void PlayDeHoverAnim(float deltaFromOriginalSize = 1) // 1 -> OriginSize
    {
        RectTransform.DOScale(deltaFromOriginalSize, _hoverSetting.scaleTransition).SetEase(Ease.OutBack);
    }
    #endregion

    #region PointUp / PointDown / Click Animation
    protected virtual void PlayPointerDown()
    {
        if (_clickSetting.IsApplyScaleAnimation)
            RectTransform.DOScale(_clickSetting.scaleOnSelect, _clickSetting.scaleTransition).SetEase(Ease.OutBack);

        ShadowRectTransform.localPosition += (-Vector3.up * _clickSetting.shadowOffset);
    }

    Vector2 _originShadowPosition = Vector2.zero;
    protected virtual void PlayPointerUp(float deltaFromOriginalSize = 1) // 1 -> OriginSize
    {
        if (_clickSetting.IsApplyScaleAnimation)
            RectTransform.DOScale(deltaFromOriginalSize, _clickSetting.scaleTransition).SetEase(Ease.OutBack);

        ShadowRectTransform.localPosition = _originShadowPosition;

        PlayIdleAnim();
    }
    #endregion

    #endregion

    #region Event Handle
    protected virtual void OnPointerEnter(PointerEventData evt)
    {
        if (HasMoved()) return;

        Card.CardState = ECardState.Hover;

        Debug.Log("On Pointer Enter");
    }

    protected virtual void OnPointerExit(PointerEventData evt)
    {
        if (HasMoved()) return;

        Card.CardState = ECardState.Idle;

        Debug.Log("On Pointer Exit");
    }

    protected Vector3 _dragOffset;
    private float _pointerDownTime;
    private bool _IsLongPress = false;
    protected virtual void OnPointerDown(PointerEventData evt)
    {
        Card.CardState = ECardState.PointDown;

        _dragOffset = RectTransform.position - (Vector3)evt.position;

        _pointerDownTime = Time.time;
    }

    protected virtual void OnPointerUp(PointerEventData evt)
    {
        RectTransform.position = Card.OriginalPosition;
        GetImage((int)Images.CardButton).raycastTarget = true;

        float pointerUpTime = Time.time;
        _IsLongPress = (pointerUpTime - _pointerDownTime > 0.2f);

        if (_IsLongPress == false) return;

        Card.CardState = ECardState.PointUp;

        Debug.Log("On Pointer Up");
    }

    protected virtual void OnClick(PointerEventData evt)
    {
        if (HasMoved()) return;
        if (_IsLongPress) return;

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
        bool hasMoved = Card.OriginalPosition != RectTransform.position;
        return hasMoved;
    }
    #endregion
}
