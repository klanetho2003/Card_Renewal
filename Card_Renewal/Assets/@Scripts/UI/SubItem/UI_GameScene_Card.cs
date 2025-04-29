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
    public Vector3 OriginalPosition { get; set; }

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

        card.OnStateChanged += OnStateChanged;
    }

    void OnStateChanged(ECardState state)
    {
        // To Do
    }

    private bool TrySwap(PointerEventData evt, bool isBoth)
    {
        if (evt.pointerEnter == null) return false;

        var other = evt.pointerEnter.GetComponentInParent<UI_GameScene_Card>();
        if (other == null) return false;
        if (other.Card.CardState == ECardState.Dragging) return false; // 처음은 other가 아닐 수 있음

        Managers.CardManager.SwapCards(this, other, isBoth);

        return true;
    }

    bool IsChangePosition()
    {
        return OriginalPosition != RectTransform.position;
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
        OriginalPosition = RectTransform.position;
        Vector3 touchPosition = evt.position;

        _dragoffset = OriginalPosition - touchPosition; // 중심과 떨어진 거리

        Card.CardState = ECardState.Dragging;

        Debug.Log("On Begin Drag");
    }

    void OnDragCardButton(PointerEventData evt)
    {
        GetImage((int)Images.CardButton).raycastTarget = false;

        TrySwap(evt, isBoth: false);

        Vector3 touchPosition = evt.position;
        RectTransform.position = touchPosition + _dragoffset;

        Debug.Log($"On Drag");
    }

    void OnUpCardButton(PointerEventData evt)
    {
        if (IsChangePosition())
        {
            if (!TrySwap(evt, isBoth: true))
                RectTransform.position = OriginalPosition; // 실패 시 복귀

            Debug.Log("On Up Button");
        }

        Card.CardState = ECardState.Idle;
        GetImage((int)Images.CardButton).raycastTarget = true;
    }
    #endregion
}
