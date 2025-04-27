using UnityEngine;
using UnityEngine.EventSystems;
using static Define;
using static UnityEditor.Progress;

public class UI_GameScene_Card : UI_Base
{
    enum Buttons
    {
        CardButton,
    }

    enum Images
    {
        CardButton,
    }

    private RectTransform _rectTransform;

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

    public void SetInfo()
    {
        transform.localScale = Vector3.one;
    }

    void OnClickCardButton(PointerEventData evt)
    {
        if (_originalPosition != _rectTransform.position)
            return;

        Debug.Log("On Click");
    }

    private Vector3 _originalPosition;
    void OnBeginDragCardButton(PointerEventData evt)
    {
        _originalPosition = _rectTransform.position;

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

        if (_originalPosition == _rectTransform.position)
            return;

        Debug.Log("On Up Button");
    }
}
