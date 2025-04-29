using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;
using static UnityEditor.Progress;

public class UI_GameScene_Card : UI_Base
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
    
    public Card Card { get; private set; }

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
            OnClickCardButton,  OnBeginDragCardButton,  OnDragCardButton,   OnUpCardButton,
            UIEvent.Click,      UIEvent.PointerDown,    UIEvent.Drag,       UIEvent.PointerUp
            );
        #endregion

        return true;
    }

    public void SetInfo(Card card)
    {
        transform.localScale = Vector3.one;

        Card = card;

        GetImage((int)Images.CardImage).sprite = Managers.Resource.Load<Sprite>(card.CardData.FrontSpriteName);

        
        gameObject.SetActive(true);
        StartCoroutine(CaptureOriginalNextFrame());

        card.CardUI = this;

        card.OnStateChanged += OnStateChanged;
    }

    // GridLayGroup 초기화 이슈, 1프레임 뒤에 Position Setting
    IEnumerator CaptureOriginalNextFrame()
    {
        yield return new WaitForEndOfFrame();    // O(1)
        Card.OriginalPosition = RectTransform.position;
    }

    void OnStateChanged(ECardState state)
    {
        // To Do
    }

    bool IsChangePosition()
    {
        return Card.OriginalPosition != RectTransform.position;
    }

    #region Event Handle
    void OnClickCardButton(PointerEventData evt)
    {
        if (IsChangePosition())
            return;

        Managers.CardManager.AddCard(TempCard_Heart_Q);

        Card.CardState = ECardState.Idle; // To Do : Select State

        Debug.Log("On Click");
    }

    Vector3 _dragoffset;
    void OnBeginDragCardButton(PointerEventData evt)
    {
        Vector3 touchPosition = evt.position;

        _dragoffset = Card.OriginalPosition - touchPosition; // 중심과 떨어진 거리

        Card.CardState = ECardState.Dragging;

        Debug.Log("On Begin Drag");
    }

    #region Swap by X Position Version
    private bool TrySwapVer2(Vector3 moveDir, PointerEventData evt, UI_GameScene_Card other, bool isBoth)
    {
        if (evt.pointerEnter == null) return false;
        if (other == null) return false;
    
        bool canSwap = false;
        if (moveDir.x < 0)
            canSwap = evt.position.x - other.Card.OriginalPosition.x < 0;
        else if (moveDir.x > 0)
            canSwap = evt.position.x - other.Card.OriginalPosition.x > 0;
    
        if (canSwap)
            Managers.CardManager.SwapCardsVer2(this, other, isBoth);
    
        return true;
    }

    Vector3 moveDir = Vector3.zero;
    Card targetCard = null;
    void OnDragCardButton(PointerEventData evt)
    {
        GetImage((int)Images.CardButton).raycastTarget = false;
    
        Vector3 touchPosition = evt.position;
        RectTransform.position = touchPosition + _dragoffset;
    
        moveDir = (touchPosition - Card.OriginalPosition).normalized;
        if (moveDir.x > 0)
            targetCard = (Card.Order < Managers.CardManager.Cards.Count-1) ? Managers.CardManager.Cards[Card.Order + 1] : null;
        else
            targetCard = (Card.Order > 0) ? Managers.CardManager.Cards[Card.Order - 1] : null;

        if (targetCard == null)
            return;

        TrySwapVer2(moveDir, evt, targetCard.CardUI, false);
    
        Debug.Log($"On Drag");
    }
    #endregion

    #region Hover Swap Version
    // private bool TrySwap(PointerEventData evt, bool isBoth)
    // {
    //     if (evt.pointerEnter == null) return false;
    // 
    //     var other = evt.pointerEnter.GetComponentInParent<UI_GameScene_Card>();
    //     if (other == null) return false;
    //     if (other.Card.CardState == ECardState.Dragging) return false; // 처음은 other가 아닐 수 있음
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

    void OnUpCardButton(PointerEventData evt)
    {
        if (IsChangePosition())
        {
            // if (!TrySwap(evt, isBoth: true))
            //     RectTransform.position = Card.OriginalPosition; // 실패 시 복귀

            if (targetCard != null)
                TrySwapVer2(moveDir, evt, targetCard.CardUI, isBoth: true);

            Debug.Log("On Up Button");
        }

        Card.CardState = ECardState.Idle;
        RectTransform.position = Card.OriginalPosition;
        GetImage((int)Images.CardButton).raycastTarget = true;
    }
    #endregion
}
