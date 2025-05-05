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

        return true;
    }

    public override void SetInfo(MatchCard card)
    {
        base.SetInfo(card);

        card.CardUI = this;
        card.OnStateChanged += OnStateChanged;
    }

    private bool TrySwap(PointerEventData evt, bool isBoth)
    {
        if (evt.pointerEnter == null)
            return false;

        if (base.TrySwap(isBoth) == false)
            return false;

        var other = evt.pointerEnter.GetComponentInParent<UI_GameSceneMatch_Card>();
        if (other == null) return false;
        if (other.Card.CardState == ECardState.Moving)
            return false;

        _cardManager.SwapMatchCards(this, other, isBoth);

        return true;
    }

    #region Event Handle
    protected override void OnPointerEnter(PointerEventData evt)
    {
        base.OnPointerEnter(evt);
    }

    protected override void OnPointerExit(PointerEventData evt)
    {
        base.OnPointerExit(evt);
    }

    protected override void OnPointerDown(PointerEventData evt)
    {
        base.OnPointerDown(evt);
    }

    protected override void OnPointerUp(PointerEventData evt)
    {
        if (Util.IsMagnitudeEqual(Card.OriginalPosition, SystemRectTransform.position))
        {
            if (!TrySwap(evt, isBoth: true))
                SystemRectTransform.position = base.Card.OriginalPosition;

            Debug.Log("On Up Button");
        }

        base.OnPointerUp(evt);
    }

    protected override void OnClick()
    {
        base.OnClick();
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
        base.OnEndDrag(evt);
    }
    #endregion
}
