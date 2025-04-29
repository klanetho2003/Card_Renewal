using System;
using System.Collections.Generic;
using Data;
using NUnit.Framework.Interfaces;
using UnityEngine;
using static Define;

public class CardManager
{
    public List<PvpCard> PvpCards { get; } = new List<PvpCard>();
    public List<MatchCard> MatchCards { get; } = new List<MatchCard>();

    public event Action OnChangePvpCardCount;
    public event Action OnChangeMatchCardCount;

    public void AddPvpCard(int TemplateId)
    {
        // PvpCard Data 생성
        var newCard = new PvpCard(TemplateId, PvpCards.Count);
        PvpCards.Add(newCard);

        OnChangePvpCardCount?.Invoke();
    }

    public void AddMatchCard(int TemplateId)
    {
        // MatchCard Data 생성
        var newCard = new MatchCard(TemplateId, MatchCards.Count);
        MatchCards.Add(newCard);

        OnChangeMatchCardCount?.Invoke();
    }

    /// <summary>
    /// pvp 카드 대전
    /// </summary>
    public void SwapCardsPVP(UI_GameScenePVP_Card selected, UI_GameScenePVP_Card target, bool updateBothPositions = true)
    {
        int idxA = selected.Card.Order;
        int idxB = target.Card.Order;
        if (idxA == idxB) return;

        // 1) PvpCard Swap
        PvpCard tmpModel = PvpCards[idxA];
        PvpCards[idxA] = PvpCards[idxB];
        PvpCards[idxB] = tmpModel;

        // 2) Order Swap
        selected.Card.Order = idxB;
        target.Card.Order   = idxA;

        // 3) OriginalPosition Swap
        Vector3 tmpPos = selected.Card.OriginalPosition;
        selected.Card.OriginalPosition = target.Card.OriginalPosition;
        target.Card.OriginalPosition   = tmpPos;

        // 4) UI Posiion 갱신
        if (updateBothPositions)
        {
            selected.RectTransform.position = selected.Card.OriginalPosition;
        }
        target.RectTransform.position = target.Card.OriginalPosition;
    }

    /// <summary>
    /// 3 Match Puzzle
    /// </summary>
    public void SwapCardsMATCH(UI_GameSceneMatch_Card selected, UI_GameSceneMatch_Card target, bool updateBothPositions = true)
    {
        int idxA = selected.Card.Order;
        int idxB = target.Card.Order;
        if (idxA == idxB) return;

        // 1) MatchCard Swap
        MatchCard tmpModel = MatchCards[idxA];
        MatchCards[idxA] = MatchCards[idxB];
        MatchCards[idxB] = tmpModel;

        // 2) Order Swap
        selected.Card.Order = idxB;
        target.Card.Order = idxA;

        // 3) OriginalPosition Swap
        Vector3 tmpPos = selected.Card.OriginalPosition;
        selected.Card.OriginalPosition = target.Card.OriginalPosition;
        target.Card.OriginalPosition = tmpPos;

        // 4) UI Posiion 갱신
        if (updateBothPositions)
        {
            selected.RectTransform.position = selected.Card.OriginalPosition;
        }
        target.RectTransform.position = target.Card.OriginalPosition;
    }
}
