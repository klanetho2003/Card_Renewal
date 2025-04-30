using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_GameSceneMatch_Card : UI_GameScene_CardBase<MatchCard>
{
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

    public override void SetInfo(MatchCard card)
    {
        base.SetInfo(card);

        card.CardUI = this;
        card.OnStateChanged += OnStateChanged;
    }

    protected override bool TrySwap(PointerEventData evt, bool isBoth)
    {
        base.TrySwap(evt, isBoth);

        var other = evt.pointerEnter.GetComponentInParent<UI_GameSceneMatch_Card>();
        if (other == null) return false;
        if (other.Card.CardState == ECardState.Dragging) return false; // 처음은 other가 아닐 수 있음

        _cardManager.SwapCardsMATCH(this, other, isBoth);

        return true;
    }

    #region Event Handle
    protected override void OnClick(PointerEventData evt)
    {
        base.OnClick(evt);
    }

    protected override void OnBeginDrag(PointerEventData evt)
    {
        base.OnBeginDrag(evt);
    }

    protected override void OnDrag(PointerEventData evt)
    {
        base.OnDrag(evt);

        TrySwap(evt, isBoth: false);
    }

    protected override void OnEndDrag(PointerEventData evt)
    {
        if (HasMoved())
        {
            if (!TrySwap(evt, isBoth: true))
                RectTransform.position = base.Card.OriginalPosition;

            Debug.Log("On Up Button");
        }

        base.OnEndDrag(evt);
    }
    #endregion
}
