using System.Collections.Generic;
using Data;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class CardManager
{
    public List<Card> Cards { get; } = new List<Card>();

    public UI_GameScene_Card SpawnCard(int TemplateId, int slotIndex)
    {
        // 1) Card Data 생성
        var card = new Card(TemplateId);
        Cards.Add(card);

        // 2) Card UI 생성
        var cardUI = Managers.UI.MakeSubItem<UI_GameScene_Card>();

        // 3) 레이아웃에 카드 크기 주입 후 슬롯 재생성 (최초 1회 혹은 크기 변경 시)
        var rt = cardUI.GetComponent<RectTransform>();
        // LayoutManager.CardSize = rt.sizeDelta;
        // LayoutManager.GenerateSlots();

        // 4) Card UI SetInfo
        // var startSlot = LayoutManager.Slots[slotIndex];
        cardUI.SetInfo(/*model, this, startSlot*/); // SetInfo(모델·매니저·시작 슬롯)

        return cardUI;
    }

    // 슬롯 인덱스 기준으로 모델 리스트 순서 교체
    public void SwapCards(int idxA, int idxB)
    {
        var tmp = Cards[idxA];
        Cards[idxA] = Cards[idxB];
        Cards[idxB] = tmp;
    }
}
