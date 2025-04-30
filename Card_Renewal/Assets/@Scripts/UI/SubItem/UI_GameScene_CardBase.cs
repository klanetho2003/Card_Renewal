using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_GameScene_CardBase<T> : UI_Base where T : CardBase
{
    #region Enum to Bind
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

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

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
        StartCoroutine(CaptureOriginalNextFrame());
    }

    IEnumerator CaptureOriginalNextFrame()
    {
        yield return new WaitForEndOfFrame();
        Card.OriginalPosition = RectTransform.position;
    }

    protected virtual void OnStateChanged(ECardState state)
    {
        // To Do : 상태별 애니메이션 처리
    }

    protected bool HasMoved()
    {
        return Card.OriginalPosition != RectTransform.position;
    }

    protected virtual bool TrySwap(PointerEventData evt, bool isBoth)
    {
        if (evt.pointerEnter == null)
            return false;

        return true;
    }

    #region Event Handle
    protected virtual void OnClick(PointerEventData evt)
    {
        if (HasMoved()) return;
        Card.CardState = ECardState.Idle; // To Do : Select State

        Debug.Log("On Click");
    }

    protected Vector3 _dragOffset;
    protected virtual void OnBeginDrag(PointerEventData evt)
    {
        _dragOffset = RectTransform.position - (Vector3)evt.position;
        Card.CardState = ECardState.Dragging;

        Debug.Log("On Begin Drag");
    }

    protected virtual void OnDrag(PointerEventData evt)
    {
        Debug.Log($"On Drag");

        GetImage((int)Images.CardButton).raycastTarget = false;

        RectTransform.position = (Vector3)evt.position + _dragOffset;
    }

    protected virtual void OnEndDrag(PointerEventData evt)
    {
        Card.CardState = ECardState.Idle;
        RectTransform.position = Card.OriginalPosition;
        GetImage((int)Images.CardButton).raycastTarget = true;
    }
    #endregion
}
