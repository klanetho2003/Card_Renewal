using System;
using System.Collections.Generic;
using Data;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class CardManager
{
    public List<Card> Cards { get; } = new List<Card>();
    List<UI_GameScene_Card> CardUIList = new List<UI_GameScene_Card>();

    public event Action OnChangeCardCount;

    public void AddCard(int TemplateId)
    {
        // Card Data 생성
        var card = new Card(TemplateId);
        Cards.Add(card);

        OnChangeCardCount.Invoke();
    }

    // 슬롯 인덱스 기준으로 모델 리스트 순서 교체
    public void SwapCards(int idxA, int idxB)
    {
        // Card Data Swap
        var tmpCard = Cards[idxA];
        Cards[idxA] = Cards[idxB];
        Cards[idxB] = tmpCard;

        // Card UI Swap
        var tmpCardUI = CardUIList[idxA];
        CardUIList[idxA] = CardUIList[idxB];
        CardUIList[idxB] = tmpCardUI;
    }
}
