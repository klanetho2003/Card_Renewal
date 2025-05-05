using System;
using System.Data;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using static Define;

public class CardBase
{
    public CardData CardData { get; protected set; }
    public TeamData TeamData { get; protected set; }

    public ECardNum CardNum { get; protected set; } = ECardNum.None;
    public ECardShape CardShape { get; protected set; } = ECardShape.None;

    public ETeamColor TeamColor { get; protected set; } = ETeamColor.None;

    public int Order { get; set; }
    public bool IsSelected { get; set; } = false;

    public Vector3 OriginalPosition { get; set; }

    public event Action<ECardState> OnStateChanged;

    private ECardState _cardState;
    public ECardState CardState
    {
        get { return _cardState; }
        set
        {
            switch (value)
            {
                case ECardState.Idle:
                case ECardState.Hover:
                case ECardState.Moving:
                    {
                        if (IsSelected == true)
                            return;
                    }
                    break;

                case ECardState.PointDown:
                case ECardState.Select:
                    break;
            }

            if (_cardState == value)
                return;

            _cardState = value;

            OnStateChanged?.Invoke(value);
        }
    }

    public CardBase(int templateId, int order)
    {
        Init(templateId, order);
    }

    public virtual bool Init(int templateId, int order)
    {
        CardState = ECardState.Idle;

        // Data
        CardData = Managers.Data.CardDic[templateId];
        CardNum = CardData.CardNUm;
        CardShape = CardData.CardShape;

        TeamColor = Managers.Game.PlayerTeamColor;
        TeamData = Managers.Data.TeamDic[(int)TeamColor];

        Order = order;

        return true;
    }
}

public class PvpCard : CardBase
{
    public UI_GameScenePVP_Card CardUI { get; set; }

    public PvpCard(int templateId, int order) : base(templateId, order)
    {
        Init(templateId, order);
    }

    public override bool Init(int templateId, int order)
    {
        if (base.Init(templateId, order) == false)
            return false;


        return true;
    }
}

public class MatchCard : CardBase
{
    public UI_GameSceneMatch_Card CardUI { get; set; }

    public MatchCard(int templateId, int order) : base(templateId, order)
    {
        Init(templateId, order);
    }

    public override bool Init(int templateId, int order)
    {
        if (base.Init(templateId, order) == false)
            return false;


        return true;
    }
}
