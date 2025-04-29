using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;
using static UnityEditor.Progress;

public class UI_GameScenePVP_Card : UI_GameScene_CardBase
{
    #region Enum to Bind
    enum Buttons
    {
        CardButton,
    }

    enum Images
    {
        CardButton,
        CardImage,
    }
    #endregion
    
    public PvpCard Card { get; private set; }

    CardManager _cardManager { get { return Managers.CardManager; } }

    public RectTransform RectTransform { get; private set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        RectTransform = GetComponent<RectTransform>();

        #region Bind
        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));
        #endregion

        #region Event Bind
        GetButton((int)Buttons.CardButton).gameObject.BindEvent
            (
            OnClick,  OnBeginDrag,  OnDrag,   OnEndDrag,
            UIEvent.Click,      UIEvent.PointerDown,    UIEvent.Drag,       UIEvent.PointerUp
            );
        #endregion

        return true;
    }

    public void SetInfo(PvpCard card)
    {
        transform.localScale = Vector3.one;

        Card = card;

        // Setting UI Image
        GetImage((int)Images.CardImage).sprite = Managers.Resource.Load<Sprite>(card.CardData.FrontSpriteName);

        // SetActive -> 1프레임 뒤에 Position Setting // GridLayGroup 초기화 이슈,
        gameObject.SetActive(true);
        StartCoroutine(CaptureOriginalNextFrame());

        card.CardUI = this;
        card.OnStateChanged += OnStateChanged;
    }

    IEnumerator CaptureOriginalNextFrame()
    {
        yield return new WaitForEndOfFrame();
        Card.OriginalPosition = RectTransform.position;
    }

    void OnStateChanged(ECardState state)
    {
        // To Do : 상태별 애니메이션 처리
    }

    bool HasMoved()
    {
        return Card.OriginalPosition != RectTransform.position;
    }
    private bool TrySwap(Vector3 moveDir, PointerEventData evt, UI_GameScenePVP_Card other, bool isBoth)
    {
        if (other == null) return false;

        float remainDestX = evt.position.x - other.Card.OriginalPosition.x; // 목적지 기준 거리 계산
        bool canSwap = (moveDir.x < 0) ? remainDestX < 0 : remainDestX > 0; // 이동 방향에 따라 Swap 가능 여부 처리

        if (canSwap)
            _cardManager.SwapCardsPVP(this, other, isBoth);

        return true;
    }

    #region Event Handle
    void OnClick(PointerEventData evt)
    {
        if (HasMoved()) return;
        Card.CardState = ECardState.Idle; // To Do : Select State

        Debug.Log("On Click");
    }

    Vector3 _dragOffset;
    void OnBeginDrag(PointerEventData evt)
    {
        _dragOffset = RectTransform.position - (Vector3)evt.position;
        Card.CardState = ECardState.Dragging;

        Debug.Log("On Begin Drag");
    }

    Vector3 _moveDir = Vector3.zero;
    PvpCard _swapTarget = null;
    void OnDrag(PointerEventData evt)
    {
        Debug.Log($"On Drag");

        GetImage((int)Images.CardButton).raycastTarget = false;
    
        // Move
        Vector3 touchPosition = evt.position;
        RectTransform.position = touchPosition + _dragOffset;

        // Swap
        _moveDir = (touchPosition - Card.OriginalPosition).normalized;
        int neighborOrder = Card.Order + (_moveDir.x > 0 ? 1 : -1);
        _swapTarget = (neighborOrder >= 0 && neighborOrder < _cardManager.PvpCards.Count) ? _cardManager.PvpCards[neighborOrder] : null;

        if (_swapTarget != null)
            TrySwap(_moveDir, evt, _swapTarget.CardUI, false);
    }

    void OnEndDrag(PointerEventData evt)
    {
        if (HasMoved())
        {
            Debug.Log("On Up Button");
        }

        Card.CardState = ECardState.Idle;
        RectTransform.position = Card.OriginalPosition;
        GetImage((int)Images.CardButton).raycastTarget = true;
    }
    #endregion

    #region Hover Swap Version
    /// 사용을 원할 시 리펙토링 필요

    // private bool TrySwap(PointerEventData evt, bool isBoth)
    // {
    //     if (evt.pointerEnter == null) return false;
    // 
    //     var other = evt.pointerEnter.GetComponentInParent<UI_GameScenePVP_Card>();
    //     if (other == null) return false;
    //     if (other.PvpCard.CardState == ECardState.Dragging) return false; // 처음은 other가 아닐 수 있음
    // 
    //     Managers.CardManager.SwapCards(this, other, isBoth);
    // 
    //     return true;
    // }
    // 
    // void OnDragCardButton(PointerEventData evt)
    // {
    //     GetImage((int)Images.CardButton).raycastTarget = false;
    // 
    //     TrySwap(evt, isBoth: false);
    // 
    //     Vector3 touchPosition = evt.position;
    //     RectTransform.position = touchPosition + _dragoffset;
    // 
    //     Debug.Log($"On Drag");
    // }
    #endregion
}
