using System;
using System.Data;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using static Define;

public class Card
{
    public CardData CardData { get; private set; }
    public TeamData TeamData { get; private set; }

    public ECardNum CardNum { get; private set; } = ECardNum.None;
    public ECardShape CardShape { get; private set; } = ECardShape.None;

    public ETeamColor TeamColor { get; private set; } = ETeamColor.None;

    public int Order { get; set; }

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

    public Card(int templateId, int order)
    {
        Init(templateId, order);
    }

    void Init(int templateId, int order)
    {
        CardState = ECardState.Idle;

        // Data
        CardData = Managers.Data.CardDic[templateId];
        CardNum = CardData.CardNUm;
        CardShape = CardData.CardShape;
         
        TeamColor = Managers.Game.PlayerTeamColor;
        TeamData = Managers.Data.TeamDic[(int)TeamColor];

        Order = order;
    }
}
