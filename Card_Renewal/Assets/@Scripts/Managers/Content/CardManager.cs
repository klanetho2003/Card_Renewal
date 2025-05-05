using System;
using System.Collections.Generic;
using Data;
using NUnit.Framework.Interfaces;
using UnityEngine;
using static Define;

public class CardManager
{
    public CardBase HoldCard { get; set; } = null; // Mouse로 Hold 중인 Card

    public List<PvpCard> PvpCards { get; } = new List<PvpCard>();
    public List<MatchCard> MatchCards { get; } = new List<MatchCard>();

    public event Action OnChangePvpCardCount;
    public event Action OnChangeMatchCardCount;

    #region Add Card
    public void AddPvpCard(int templateId)
    {
        AddCard(PvpCards, (id, order) => new PvpCard(id, order), templateId, OnChangePvpCardCount);
    }

    public void AddMatchCard(int templateId)
    {
        AddCard(MatchCards, (id, order) => new MatchCard(id, order), templateId, OnChangeMatchCardCount);
    }

    private void AddCard<T>(List<T> list, Func<int, int, T> cardFactory, int templateId, Action onChange)
    {
        var newCard = cardFactory.Invoke(templateId, list.Count);
        list.Add(newCard);
        onChange?.Invoke();
    }
    #endregion

    #region Swap Card
    public void SwapPvpCards(UI_GameScenePVP_Card selected, UI_GameScenePVP_Card target, bool updateBoth = true)
    {
        SwapCards(PvpCards, selected, target, updateBoth);
    }

    public void SwapMatchCards(UI_GameSceneMatch_Card selected, UI_GameSceneMatch_Card target, bool updateBoth = true)
    {
        SwapCards(MatchCards, selected, target, updateBoth);
    }

    private void SwapCards<TCard, TUI>(List<TCard> list, TUI selected, TUI target, bool updateBoth) where TCard : CardBase where TUI : UI_GameScene_CardBase<TCard>
    {
        if (Util.IsMagnitudeEqual(selected.Card.OriginalPosition, target.Card.OriginalPosition))
            return;

        int idxA = selected.Card.Order;
        int idxB = target.Card.Order;
        if (idxA == idxB) return;

        // CardList 순서 Swap
        (list[idxA], list[idxB]) = (list[idxB], list[idxA]);

        // Card가 지닌 순서 Data Swap
        (selected.Card.Order, target.Card.Order) = (idxB, idxA);
        target.SetCanvas(target.CardCanvas, target.Card.Order);

        // Card가 지닌 위치값 Swap
        (selected.Card.OriginalPosition, target.Card.OriginalPosition) =
            (target.Card.OriginalPosition, selected.Card.OriginalPosition);

        // UI 갱신
        if (updateBoth)
        {
            selected.UpdatePositionFromCard();
            selected.SetCanvas(selected.CardCanvas, selected.Card.Order);
        }

        target.UpdatePositionFromCard();
    }
    #endregion
}
