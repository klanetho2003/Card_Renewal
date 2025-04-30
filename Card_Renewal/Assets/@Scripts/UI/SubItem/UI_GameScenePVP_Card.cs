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

    public override void SetInfo(PvpCard card)
    {
        base.SetInfo(card);

        card.CardUI = this;
        card.OnStateChanged += OnStateChanged;
    }
    
    private bool TrySwap(Vector3 moveDir, PointerEventData evt, UI_GameScenePVP_Card other, bool isBoth)
    {
        if (base.TrySwap(evt, isBoth) == false)
            return false;

        float remainDestX = evt.position.x - other.Card.OriginalPosition.x; // 목적지 기준 거리 계산
        bool canSwap = (moveDir.x < 0) ? remainDestX < 0 : remainDestX > 0; // 이동 방향에 따라 Swap 가능 여부 처리

        if (canSwap)
            _cardManager.SwapPvpCards(this, other, isBoth);

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

    Vector3 _moveDir = Vector3.zero;
    PvpCard _swapTarget = null;
    protected override void OnDrag(PointerEventData evt)
    {
        base.OnDrag(evt);

        // Swap
        _moveDir = ((Vector3)evt.position - Card.OriginalPosition).normalized;
        int neighborOrder = Card.Order + (_moveDir.x > 0 ? 1 : -1);
        _swapTarget = (neighborOrder >= 0 && neighborOrder < _cardManager.PvpCards.Count) ? _cardManager.PvpCards[neighborOrder] : null;

        if (_swapTarget != null)
            TrySwap(_moveDir, evt, _swapTarget.CardUI, false);
    }

    protected override void OnEndDrag(PointerEventData evt)
    {
        if (HasMoved())
        {
            Debug.Log("On Up Button");
        }

        base.OnEndDrag(evt);
    }
    #endregion
}
