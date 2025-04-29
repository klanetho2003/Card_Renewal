using System;
using System.Collections.Generic;
using Data;
using NUnit.Framework.Interfaces;
using UnityEngine;
using static Define;

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
        var tmpPos = selected.Card.OriginalPosition;
        if (updateBothPositions)
            selected.RectTransform.position = target.RectTransform.position;
        else
            selected.Card.OriginalPosition = target.RectTransform.position;
        target.RectTransform.position = tmpPos;
    }

    public void SwapCardsVer2(UI_GameScene_Card selected, UI_GameScene_Card target, bool updateBothPositions = true)
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
        /*var tmpPos = selected.Card.OriginalPosition;
        if (updateBothPositions)
            selected.RectTransform.position = target.RectTransform.position;
        else
            selected.Card.OriginalPosition = target.RectTransform.position;
        target.RectTransform.position = tmpPos;*/


        var tmpOriginPos = selected.Card.OriginalPosition;
        selected.Card.OriginalPosition = target.Card.OriginalPosition;
        target.Card.OriginalPosition = tmpOriginPos;

        if (updateBothPositions)
            selected.RectTransform.position = selected.Card.OriginalPosition;
        target.RectTransform.position = target.Card.OriginalPosition;
    }
}
