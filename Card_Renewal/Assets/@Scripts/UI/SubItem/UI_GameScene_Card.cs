using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;
using static UnityEditor.Progress;

public class UI_GameScene_Card : UI_Base
{
    #region Bind
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

    public RectTransform RectTransform { get; private set; }
    public Card Card {  get; private set; }

    public Vector3 OriginalPosition { get; private set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        RectTransform = GetComponent<RectTransform>();

        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));

        GetButton((int)Buttons.CardButton).gameObject.BindEvent
            (
            OnClickCardButton,  OnBeginDragCardButton,  OnDragCardButton,   OnUpCardButton,
            UIEvent.Click,      UIEvent.PointerDown,    UIEvent.Drag,       UIEvent.PointerUp
            );

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

    void OnClickCardButton(PointerEventData evt)
    {
        if (IsChangePosition())
            return;

        Card.CardState = ECardState.Idle; // To Do : Select State

        Debug.Log("On Click");
    }

    Vector3 _mouseStartPos;
    void OnBeginDragCardButton(PointerEventData evt)
    {
        OriginalPosition = RectTransform.position;
        _mouseStartPos = evt.position;

        Card.CardState = ECardState.Dragging;

        Debug.Log("On Begin Drag");
    }

    void OnDragCardButton(PointerEventData evt)
    {
        GetImage((int)Images.CardButton).raycastTarget = false;

        Vector3 touchPosition = evt.position;
        RectTransform.position = OriginalPosition + (touchPosition - _mouseStartPos);

        Debug.Log($"On Drag");
    }

    void OnUpCardButton(PointerEventData evt)
    {
        if (IsChangePosition())
        {
            if (!TrySwap(evt))
                RectTransform.position = OriginalPosition; // 실패 시 복귀

            Debug.Log("On Up Button");
        }

        Card.CardState = ECardState.Idle;
        GetImage((int)Images.CardButton).raycastTarget = true;
    }

    private bool TrySwap(PointerEventData evt)
    {
        if (evt.pointerEnter == null) return false;

        var other = evt.pointerEnter.GetComponentInParent<UI_GameScene_Card>();
        if (other == null) return false;

        Managers.CardManager.SwapCards(this, other);

        return true;
    }

    bool IsChangePosition()
    {
        return OriginalPosition != RectTransform.position;
    }
}
