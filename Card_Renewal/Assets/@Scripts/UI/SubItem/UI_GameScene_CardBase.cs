using System;
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
        Card,
        CardShadow,
        CardImage,
    }

    protected enum Buttons
    {
        TouchArea,
    }

    protected enum Images
    {
        TouchArea,
        CardImage,
    }

    protected enum Canvases
    {
        Card,
    }
    #endregion

    public T Card { get; protected set; }

    protected CardManager _cardManager { get { return Managers.CardManager; } }

    public Vector2 TargetPos { get; protected set; } = Vector2.zero;

    public Canvas CardCanvas { get; protected set; }    // System적으로 사용되는 Rect - ex 카드 Swap

    public RectTransform SystemRectTransform { get; protected set; }    // System적으로 사용되는 Rect - ex 카드 Swap
    public RectTransform CardRectTransform { get; protected set; }      // ImageRectTransform & ShadowRectTransform 둘 다
    public RectTransform ImageRectTransform { get; protected set; }     // Tweening에 사용되는 Rect
    public RectTransform ShadowRectTransform { get; protected set; }    // 카드 그림자

    // Scriptable Objects
    private IdleTiltSetting _tiltSetting;
    private HoverSetting _hoverSetting;
    private ClickSetting _clickSetting;
    private MoveSetting _moveSetting;

    private CardAnimator<UI_GameScene_CardBase<T>, T> _cardAnimator;

    [SerializeField]
    ECardState Debug_CurrentState = ECardState.None;

    #region Init & SetInfo
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SystemRectTransform = GetComponent<RectTransform>();

        _tiltSetting = Managers.Resource.Load<IdleTiltSetting>("IdleTiltSetting");
        _hoverSetting = Managers.Resource.Load<HoverSetting>("HoverSetting");
        _clickSetting = Managers.Resource.Load<ClickSetting>("ClickSetting");
        _moveSetting = Managers.Resource.Load<MoveSetting>("MoveSetting");

        #region Bind
        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));
        Bind<Canvas>(typeof(Canvases));
        #endregion

        #region Event Bind
        GetButton((int)Buttons.TouchArea).gameObject.BindEvent
            (
            (UIEvent.PointerEnter,  OnPointerEnter),
            (UIEvent.PointerExit,   OnPointerExit),
            (UIEvent.PointerDown,   OnPointerDown),
            (UIEvent.PointerUp,     OnPointerUp),
            (UIEvent.BeginDrag,     OnBeginDrag),
            (UIEvent.Drag,          OnDrag),
            (UIEvent.EndDrag,       OnEndDrag)
            );
        #endregion

        return true;
    }

    public virtual void SetInfo(T card)
    {
        transform.localScale = Vector2.one;

        Card = card;

        // Setting UI Image
        GetImage((int)Images.CardImage).sprite = Managers.Resource.Load<Sprite>(card.CardData.FrontSpriteName);

        // SetActive ---1프레임_뒤에--->  Position Setting // GridLayGroup 초기화 이슈 때문,
        gameObject.SetActive(true);
        StartCoroutine(CaptureOriginalNextFrame());

        // Set Canvas
        CardCanvas = Get<Canvas>((int)Canvases.Card);
        SetCanvas(CardCanvas, Card.Order);

        // Caching
        CardRectTransform = GetObject((int)GameObjects.Card).GetComponent<RectTransform>();
        ImageRectTransform = GetObject((int)GameObjects.CardImage).GetComponent<RectTransform>();
        ShadowRectTransform = GetObject((int)GameObjects.CardShadow).GetComponent<RectTransform>();

        // Add Animtor
        _cardAnimator = new CardAnimator<UI_GameScene_CardBase<T>, T>(this,
            CardRectTransform, ImageRectTransform, ShadowRectTransform,
            _tiltSetting, _hoverSetting, _clickSetting, _moveSetting);
    }

    IEnumerator CaptureOriginalNextFrame()
    {
        yield return new WaitForEndOfFrame();
        Card.OriginalPosition = SystemRectTransform.position;
        TargetPos = SystemRectTransform.position;

        PlayAnimation(ECardState.Idle);
    }
    #endregion

    #region Update
    private void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate() { }
    #endregion

    public void UpdatePositionFromCard()
    {
        _cardAnimator.MoveTo(SystemRectTransform, Card.OriginalPosition, isRayCast: false);
    }

    protected virtual bool TrySwap(bool isBoth)
    {
        if (Managers.CardManager.HoldCard != Card)
            return false;
        // 이동 여부 Check
        if (Util.IsMagnitudeEqual(Card.OriginalPosition, SystemRectTransform.position))
            return false;

        return true;
    }

    protected virtual void OnStateChanged(ECardState currentState)
    {
        PlayAnimation(currentState);

        Debug_CurrentState = currentState;
    }

    void PlayAnimation(ECardState newState)
    {
        _cardAnimator.ResetToIdle();

        switch (newState)
        {
            case ECardState.Idle:       _cardAnimator.PlayIdle(); break;
            case ECardState.Hover:      _cardAnimator.PlayHover(); break;
            case ECardState.PointDown:  _cardAnimator.PlayPointerDown(); break;
            case ECardState.Select:     _cardAnimator.PlaySelect(); break;
            case ECardState.Moving:     _cardAnimator.PlayMoving(); break;
            // ...
        }
    }

    #region Event Handle
    protected virtual void OnPointerEnter(PointerEventData evt)
    {
        if (Managers.CardManager.HoldCard != null)
            return;
        if (Card.CardState == ECardState.Moving)
            return;

        Card.CardState = ECardState.Hover;

        Debug.Log("On Pointer Enter");
    }

    protected virtual void OnPointerExit(PointerEventData evt)
    {
        if (Card.CardState == ECardState.Moving)
            return;

        Card.CardState = ECardState.Idle;

        Debug.Log("On Pointer Exit");
    }

    public Vector2 MovementByMouse { get; protected set; } = Vector2.zero;
    public Vector2 InputPosition { get; protected set; } = Vector2.zero;
    protected Vector2 _dragOffset;
    private float _pointerDownTime;
    private bool _IsLongPress = false;
    protected virtual void OnPointerDown(PointerEventData evt)
    {
        Managers.CardManager.HoldCard = Card;

        Card.CardState = ECardState.PointDown;

        _dragOffset = ImageRectTransform.position - (Vector3)evt.position;

        _pointerDownTime = Time.time;

        SetCanvas(CardCanvas);
    }

    protected virtual void OnPointerUp(PointerEventData evt)
    {
        Managers.CardManager.HoldCard = null;

        // Reset
        TargetPos = Card.OriginalPosition;
        SetRayCastTargrt(true);

        // 1) 마우스 버튼을 누른 시간 Check
        float pointerUpTime = Time.time;
        _IsLongPress = (pointerUpTime - _pointerDownTime > 0.2f);
        if (_IsLongPress == false)
        {
            // 2) 마우스 이동 여부 Check
            if (Util.IsMagnitudeEqual(Card.OriginalPosition, SystemRectTransform.position))
            {
                OnClick();
                return;
            }
        }

        // 이미 IsSelected면 Selected 상태로 변경
        if (Card.IsSelected == true)
        {
            Card.CardState = ECardState.Select; // (PointerDown -> Select)
            return;
        }

        Card.CardState = ECardState.Idle;
        _IsLongPress = false;

        SetCanvas(CardCanvas, Card.Order);

        Debug.Log("On Pointer Up");
    }

    protected virtual void OnClick()
    {
        Debug.Log("On Click");

        if (Card.IsSelected == false)
        {
            Card.CardState = ECardState.Select;
            Card.IsSelected = true;
            return;
        }

        // Card.IsSelected == true
        {
            Card.IsSelected = false;
            Card.CardState = ECardState.Idle;
        }
    }

    protected virtual void OnBeginDrag(PointerEventData evt)
    {
        
    }

    protected virtual void OnDrag(PointerEventData evt)
    {
        Debug.Log($"On Drag");

        Card.CardState = ECardState.Moving;

        SetRayCastTargrt(false);

        TargetPos = evt.position + _dragOffset;
        InputPosition = evt.position;
        MovementByMouse = InputPosition - Card.OriginalPosition;
    }

    protected virtual void OnEndDrag(PointerEventData evt)
    {
        
    }
    #endregion

    #region Helper
    public void SetRayCastTargrt(bool setBool)
    {
        GetImage((int)Images.TouchArea).raycastTarget = setBool;
    }

    public void SetCanvas(Canvas canvas, int order = 999, bool isSorting = true)
    {
        canvas.overrideSorting = isSorting;
        canvas.sortingOrder = order;
    }
    #endregion
}

