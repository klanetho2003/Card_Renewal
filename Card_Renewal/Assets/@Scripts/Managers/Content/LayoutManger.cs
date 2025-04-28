using System.Collections.Generic;
using UnityEngine;

public class LayoutManger
{
    List<CardSlot> _slots = new List<CardSlot>();
    public List<CardSlot> Slots { get { return _slots; } }

    public CardSlot GenerateSlot(UI_GameScene_Card cardUI, int count)
    {
        Vector3 rectPos = cardUI.GetComponent<RectTransform>().anchoredPosition; // Util.TransformPoint(local3D, originPos, originRot, originScale);
        CardSlot cardSlot = new CardSlot(count, rectPos);
        _slots.Add(cardSlot);

        return cardSlot;
    }
}
