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
    public void SwapCards(UI_GameScene_Card A, UI_GameScene_Card B)
    {
        // Card Data Swap
        var tmpCard = Cards[A.Card.Order];
        Cards[A.Card.Order] = Cards[B.Card.Order];
        Cards[B.Card.Order] = tmpCard;

        // Card Order Swap
        var tmpOrder = A.Card.Order;
        A.Card.Order = B.Card.Order;
        B.Card.Order = tmpOrder;

        // Card Position Swap
        var tmpPos = A.OriginalPosition;
        A.RectTransform.position = B.RectTransform.position;
        B.RectTransform.position = tmpPos;
    }
}
