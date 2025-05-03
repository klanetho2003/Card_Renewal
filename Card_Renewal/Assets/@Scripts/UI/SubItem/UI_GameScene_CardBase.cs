using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

#region Animation Interface
public interface ICardAnimator
{
    void PlayIdle();
    void PlayHover();
    void PlayPointerDown();
    // void PlayPointerUp(); To Do  : Remove
    void PlaySelect();
    void ResetToIdle();
}

public class CardAnimator<TUI, T> : ICardAnimator where TUI : UI_GameScene_CardBase<T> where T : CardBase
{
    private readonly TUI _owner;

    private RectTransform _cardRect;
    private RectTransform _imageRect;
    private RectTransform _shadowRect;

    // ScriptableObject
    private IdleTiltSetting _tilt;
    private HoverSetting _hover;
    private ClickSetting _click;

    private Coroutine _tiltCoroutine;
    private float _tiltStartOffset;
    private Vector3 _originalShadowPos;

    public CardAnimator(TUI owner, RectTransform card, RectTransform image, RectTransform shadow,
        IdleTiltSetting tilt, HoverSetting hover, ClickSetting click)
    {
        _owner = owner;
        _cardRect = card;
        _imageRect = image;
        _shadowRect = shadow;
        _tilt = tilt;
        _hover = hover;
        _click = click;

        _originalShadowPos = shadow.localPosition;
    }

    public void PlayIdle()
    {
        _tiltStartOffset = Random.Range(0f, Mathf.PI * 2f);

        PlayTilt(_tilt.maxAngle, _tilt.lerpSpeed);
    }
    private void PlayTilt(float maxAngle, float lerpSpeed)
    {
        StopTilt();
        _tiltCoroutine = _owner.StartCoroutine(CoTilt(maxAngle, lerpSpeed));
    }

    IEnumerator CoTilt(float maxAngle, float lerpSpeed)
    {
        while (true)
        {
            float time = Time.time + _tiltStartOffset;
            float sin = Mathf.Sin(time);
            float cos = Mathf.Cos(time);

            Vector3 euler = _imageRect.eulerAngles;
            float newX = Mathf.LerpAngle(euler.x, sin * maxAngle, lerpSpeed * Time.deltaTime);
            float newY = Mathf.LerpAngle(euler.y, cos * maxAngle, lerpSpeed * Time.deltaTime);

            _imageRect.eulerAngles = new Vector3(newX, newY, 0f);
            yield return null;
        }
    }

    public void PlayHover()
    {
        if (_hover.IsApplyScaleAnimation)
            _cardRect.DOScale(_hover.scaleOnHover, _hover.scaleDuration).SetEase(Ease.OutBack);

        _imageRect
        .DOPunchRotation(Vector3.forward * _hover.hoverPunchAngle, _hover.hoverTransition, 20, 1)
        .OnComplete(() =>
        {
            // 애니메이션 끝나면 실행
            PlayTilt(_tilt.hsMaxAngle, _tilt.lerpSpeed);
        });
    }

    public void PlayPointerDown()
    {
        if (_click.IsApplyScaleAnimation)
            _cardRect.DOScale(_click.scaleOnSelect, _click.scaleTransition).SetEase(Ease.OutBack);

        _shadowRect.localPosition = _originalShadowPos + (-Vector3.up * _click.shadowOffset);
    }

    public void PlaySelect()
    {
        _cardRect.localPosition += (_cardRect.up * _click.selectPositionY_Offset);

        Sequence selectSequence = DOTween.Sequence();
        selectSequence
            .Append(_imageRect.DOPunchPosition(_cardRect.up * _click.selectPunchAmount, _click.scaleTransition))
            .Join(_imageRect.DOPunchRotation(Vector3.forward * (_hover.hoverPunchAngle / 2), _hover.hoverTransition, 20, 1))
            .OnComplete(() =>
            {
                // Card 작아지고
                _cardRect.DOScale(Vector3.one, _hover.scaleDuration).SetEase(Ease.OutBack)

                .OnComplete(() =>
                {
                    // Tilt 재생
                    PlayTilt(_tilt.hsMaxAngle, _tilt.lerpSpeed);
                });
            });

        selectSequence.Play();
    }

    public void ResetToIdle()
    {
        StopTilt();
        DOTween.Kill(_imageRect);

        if (Util.IsMagnitudeEqual(_cardRect.localScale, Vector3.one) == false)
            _cardRect.DOScale(Vector3.one, _hover.scaleDuration).SetEase(Ease.OutBack);
        if (_owner.Card.CardState != ECardState.PointDown)
            _cardRect.localPosition = Vector3.zero;

        _imageRect.localScale = Vector3.one;
        _imageRect.localPosition = Vector3.zero;
        _imageRect.rotation = Quaternion.identity;

        _shadowRect.localPosition = _originalShadowPos;
    }

    public void StopTilt()
    {
        if (_tiltCoroutine == null)
            return;

        _owner.StopCoroutine(_tiltCoroutine);
        _tiltCoroutine = null;
    }
}
#endregion

public class UI_GameScene_CardBase<T> : UI_Base where T : CardBase
{
    #region Enum to Bind
    protected enum GameObjects
    {
        Card,
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

    public RectTransform SystemRectTransform { get; protected set; }    // System적으로 사용되는 Rect - ex 카드 Swap
    public RectTransform CardRectTransform { get; protected set; }      // ImageRectTransform & ShadowRectTransform 둘 다
    public RectTransform ImageRectTransform { get; protected set; }     // Tweening에 사용되는 Rect
    public RectTransform ShadowRectTransform { get; protected set; }    // 카드 그림자

    // Scriptable Objects
    private IdleTiltSetting _tiltSetting;
    private HoverSetting _hoverSetting;
    private ClickSetting _clickSetting;

    private CardAnimator<UI_GameScene_CardBase<T>, T> _cardAnimator;

    [SerializeField]
    ECardState Debug_CurrentState = ECardState.None;
    [SerializeField]
    ECardState Debug_LastState = ECardState.None;

    #region Init & SetInfo
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SystemRectTransform = GetComponent<RectTransform>();

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

        // SetActive ---1프레임_뒤에--->  Position Setting // GridLayGroup 초기화 이슈 때문,
        gameObject.SetActive(true);
        StartCoroutine(CaptureOriginalNextFrame());

        // Caching
        CardRectTransform = GetObject((int)GameObjects.Card).GetComponent<RectTransform>();
        ImageRectTransform = GetObject((int)GameObjects.CardButton).GetComponent<RectTransform>();
        ShadowRectTransform = GetObject((int)GameObjects.CardShadow).GetComponent<RectTransform>();

        // Add Animtor
        _cardAnimator = new CardAnimator<UI_GameScene_CardBase<T>, T>(this,
            CardRectTransform, ImageRectTransform, ShadowRectTransform,
            _tiltSetting, _hoverSetting, _clickSetting);
    }

    IEnumerator CaptureOriginalNextFrame()
    {
        yield return new WaitForEndOfFrame();
        Card.OriginalPosition = SystemRectTransform.position;

        PlayAnimation(ECardState.Idle, ECardState.None);
    }
    #endregion

    public void UpdatePositionFromCard()
    {
        SystemRectTransform.position = Card.OriginalPosition;
    }

    protected virtual bool TrySwap(PointerEventData evt, bool isBoth)
    {
        if (Util.IsMagnitudeEqual(Card.OriginalPosition, SystemRectTransform.position))
            return false;

        if (evt.pointerEnter == null)
            return false;

        return true;
    }

    protected virtual void OnStateChanged(ECardState currentState, ECardState lastState)
    {
        PlayAnimation(currentState, lastState);

        Debug_CurrentState = currentState;
        Debug_LastState = lastState;
    }

    void PlayAnimation(ECardState newState, ECardState lastState)
    {
        _cardAnimator.ResetToIdle();

        switch (newState)
        {
            case ECardState.Idle:       _cardAnimator.PlayIdle(); break;
            case ECardState.Hover:      _cardAnimator.PlayHover(); break;
            case ECardState.PointDown:  _cardAnimator.PlayPointerDown(); break;
            case ECardState.Select:     _cardAnimator.PlaySelect(); break;
            // ...
        }
    }

    #region Event Handle
    protected virtual void OnPointerEnter(PointerEventData evt)
    {
        if (Util.IsMagnitudeEqual(Card.OriginalPosition, SystemRectTransform.position) == false)
            return;

        Card.CardState = ECardState.Hover;

        Debug.Log("On Pointer Enter");
    }

    protected virtual void OnPointerExit(PointerEventData evt)
    {
        if (Util.IsMagnitudeEqual(Card.OriginalPosition, SystemRectTransform.position) == false)
            return;

        Card.CardState = ECardState.Idle;

        Debug.Log("On Pointer Exit");
    }

    protected Vector3 _dragOffset;
    private float _pointerDownTime;
    private bool _IsLongPress = false;
    protected virtual void OnPointerDown(PointerEventData evt)
    {
        Card.CardState = ECardState.PointDown;

        _dragOffset = ImageRectTransform.position - (Vector3)evt.position;

        _pointerDownTime = Time.time;
    }

    protected virtual void OnPointerUp(PointerEventData evt)
    {
        // Reset
        SystemRectTransform.position = Card.OriginalPosition;
        GetImage((int)Images.CardButton).raycastTarget = true;

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

        GetImage((int)Images.CardButton).raycastTarget = false;

        SystemRectTransform.position = (Vector3)evt.position + _dragOffset;
    }

    protected virtual void OnEndDrag(PointerEventData evt)
    {
        
    }
    #endregion
}

