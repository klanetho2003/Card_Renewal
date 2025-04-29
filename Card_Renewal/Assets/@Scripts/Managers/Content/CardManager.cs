using System;
using System.Collections.Generic;
using Data;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class CardManager
{
    public List<Card> Cards { get; } = new List<Card>();

    public event Action OnChangeCardCount;

    public void AddCard(int TemplateId)
    {
        // Card Data 생성
        var card = new Card(TemplateId, Cards.Count);
        Cards.Add(card);

        OnChangeCardCount?.Invoke();
    }

    // 슬롯 인덱스 기준으로 모델 리스트 순서 교체
    public void SwapCards(UI_GameScene_Card selected, UI_GameScene_Card target, bool updateBothPositions = true)
    {
        // Card Data Swap
        var tmpCard = Cards[selected.Card.Order];
        Cards[selected.Card.Order] = Cards[target.Card.Order];
        Cards[target.Card.Order] = tmpCard;

        // Card Order Swap
        var tmpOrder = selected.Card.Order;
        selected.Card.Order = target.Card.Order;
        target.Card.Order = tmpOrder;

        // Card Position Swap
        var tmpPos = selected.OriginalPosition;
        if (updateBothPositions)
            selected.RectTransform.position = target.RectTransform.position;
        else
            selected.OriginalPosition = target.RectTransform.position;
        target.RectTransform.position = tmpPos;
    }
}
