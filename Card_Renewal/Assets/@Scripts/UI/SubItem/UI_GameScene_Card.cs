using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

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
    }
    #endregion

    private RectTransform _rectTransform;
    Card _card;
    CardSlot _slot;
    Vector3 _dragOffset;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _rectTransform = GetComponent<RectTransform>();

        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));

        GetButton((int)Buttons.CardButton).gameObject.BindEvent(OnClickCardButton, OnBeginDragCardButton, OnDragCardButton, OnUpCardButton, UIEvent.Click, UIEvent.PointerDown, UIEvent.Drag, UIEvent.PointerUp);

        return true;
    }

    public void SetInfo(Card card, CardSlot slot)
    {
        transform.localScale = Vector3.one;

        _card = card;
        _slot = slot;

        transform.position = slot.RectPosition;

        card.OnStateChanged += OnStateChanged;
    }

    void OnStateChanged(ECardState state)
    {
        // To Do
    }

    void OnClickCardButton(PointerEventData evt)
    {
        // Is Drag
        if (_originalPosition != _rectTransform.position)
            return;

        Debug.Log("On Click");
    }

    private Vector3 _originalPosition;
    void OnBeginDragCardButton(PointerEventData evt)
    {
        _originalPosition = _rectTransform.position;

        _card.CardState = ECardState.Dragging;

        Debug.Log("On Begin Drag");
    }

    void OnDragCardButton(PointerEventData evt)
    {
        GetImage((int)Images.CardButton).raycastTarget = false;

        Vector2 touchPosition = evt.position;
        _rectTransform.position = touchPosition;

        Debug.Log($"On Drag");
    }

    void OnUpCardButton(PointerEventData evt)
    {
        GetImage((int)Images.CardButton).raycastTarget = true;

        // Click
        if (_originalPosition == _rectTransform.position)
            return;

        _card.CardState = ECardState.Idle;

        Vector2 mySize = _rectTransform.sizeDelta;
        Vector3 worldCtr = _rectTransform.TransformPoint(_rectTransform.rect.center);

        // 최적화 필요
        var target = Managers.LayoutManger.Slots
            .FirstOrDefault(s => s.GetBounds(mySize).Contains((Vector2)worldCtr));

        if (target != null && target.SlotIndex != _slot.SlotIndex)
        {
            Managers.CardManager.SwapCards(_slot.SlotIndex, target.SlotIndex);
            _slot = target;
        }

        _rectTransform.anchoredPosition = _slot.RectPosition;

        Debug.Log("On Up Button");
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
    public void UpdateOriginalSlot(CardSlot slot)
    {
        _slot = slot;
    }
}
