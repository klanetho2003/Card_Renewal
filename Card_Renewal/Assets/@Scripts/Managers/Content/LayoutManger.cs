using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class LayoutManger
{
    static readonly Vector2 SPACING = new Vector2(10, 10);
    static readonly Vector2 START_OFFSET = new Vector2(-300, 0);

    List<CardSlot> slots = new List<CardSlot>();

    // 외부에서 주입받는 카드 크기
    public Vector2 CardSize { get; set; }

    // 슬롯 생성
    public void GenerateSlots(List<Card> cards, Vector2 spacing, int XCount = 5, int YCount = 1)
    {
        slots.Clear();
        for (int y = 0; y < YCount; y++)
        {
            for (int x = 0; x < XCount; x++)
            {
                int idx = y * XCount + x;
                Card owner = cards[idx];

                slots.Add(new CardSlot(owner, idx));
            }
        }
    }

    public IReadOnlyList<CardSlot> Slots => slots;





    // public void GenerateSlots(int XCount, int YCount)
    // {
    //     slots.Clear();
    //     for (int y = 0; y < YCount; y++)
    //     {
    //         for (int x = 0; x < XCount; x++)
    //         {
    //             int idx = y * XCount + x;
    //             Vector2 localPos = START_OFFSET + new Vector2(
    //                 x * (CardSize.x + SPACING.x),
    //                 -y * (CardSize.y + SPACING.y)
    //             );
    //             Vector3 worldPos = transform.TransformPoint(localPos);
    //             slots.Add(new CardSlot(idx, worldPos));
    //         }
    //     }
    // }
}
