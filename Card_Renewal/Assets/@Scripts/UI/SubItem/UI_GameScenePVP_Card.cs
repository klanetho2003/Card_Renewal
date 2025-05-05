using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;
using static UnityEditor.Progress;

public class UI_GameScenePVP_Card : UI_GameScene_CardBase<PvpCard>
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public override void SetInfo(PvpCard card)
    {
        base.SetInfo(card);

        card.CardUI = this;
        card.OnStateChanged += OnStateChanged;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (Card.CardState != ECardState.Moving)
            return;

        int neighborOrder = Card.Order + (_moveDir.x > 0 ? 1 : -1);
        _swapTarget = (neighborOrder >= 0 && neighborOrder < _cardManager.PvpCards.Count) ? _cardManager.PvpCards[neighborOrder] : null;

        if (_swapTarget != null)
            TrySwap(_moveDir, _swapTarget.CardUI, false);
    }

    private bool TrySwap(Vector3 moveDir, UI_GameScenePVP_Card other, bool isBoth)
    {
        if (other.Card.CardState == ECardState.Moving)
            return false;

        if (base.TrySwap(isBoth) == false)
            return false;

        float remainDestX = SystemRectTransform.position.x - other.Card.OriginalPosition.x; // 목적지 기준 거리 계산
        bool canSwap = (moveDir.x < 0) ? remainDestX < 0 : remainDestX > 0; // 이동 방향에 따라 Swap 가능 여부 처리

        if (canSwap)
            _cardManager.SwapPvpCards(this, other, isBoth);

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

    Vector2 _moveDir = Vector2.zero;
    PvpCard _swapTarget = null;
    protected override void OnDrag(PointerEventData evt)
    {
        base.OnDrag(evt);

        _moveDir = (InputPosition - (Vector2)ImageRectTransform.position).normalized;
    }

    protected override void OnEndDrag(PointerEventData evt)
    {
        base.OnEndDrag(evt);
    }
    #endregion
}
