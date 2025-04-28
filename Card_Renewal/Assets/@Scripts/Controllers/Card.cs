using System;
using System.Data;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using static Define;

public class CardSlot
{
    Card _owner;
    public int SlotIndex { get; }
    public Vector3 WorldPosition { get; }

    public CardSlot(Card owner, int slotIdx)
    {
        _owner = owner;

        SlotIndex = slotIdx;
    }

    // 외부에서 size를 받아 영역 반환
    public Rect GetBounds(Vector2 size)
    {
        Vector2 half = size * 0.5f;
        Vector2 min = (Vector2)WorldPosition - half;
        return new Rect(min, size);
    }
}

public class Card
{
    public CardData CardData { get; private set; }
    public TeamData TeamData { get; private set; }

    public ECardNum CardNum { get; private set; } = ECardNum.None;
    public ECardShape CardShape { get; private set; } = ECardShape.None;

    public ETeamColor TeamColor { get; private set; } = ETeamColor.None;

    public event Action<ECardState> OnStateChanged;
    public ECardState _cardState = ECardState.None;
    public ECardState CardState
    {
        get { return _cardState; }
        set
        {
            if (_cardState == value)
                return;

            _cardState = value;

            OnStateChanged?.Invoke(value);
        }
    }
    
    public Card(int templateId)
    {
        Init(templateId);
    }

    void Init(int templateId)
    {
        CardState = ECardState.Idle;

        // Data
        CardData = Managers.Data.CardDic[templateId];
        CardNum = CardData.CardNUm;
        CardShape = CardData.CardShape;
         
        TeamColor = Managers.Game.PlayerTeamColor;
        TeamData = Managers.Data.TeamDic[(int)TeamColor];
    }
}
