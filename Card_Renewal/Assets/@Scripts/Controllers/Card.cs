using System.Data;
using Data;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class Card : BaseController
{
    public CardData CardData { get; private set; }
    public TeamData TeamData { get; private set; }

    public ECardNum CardNum { get; private set; } = ECardNum.None;
    public ECardShape CardShape{ get; private set; } = ECardShape.None;
    public ETeamColor TeamColor { get; private set; } = ETeamColor.None;

    public ECardState _cardState = ECardState.None;
    public ECardState CardState
    {
        get { return _cardState; }
        set
        {
            _cardState = value;

            UpdateAnimation();
        }
    }

    // public Sprite FrontImage { get; private set; } = null;
    public Sprite BackImage { get; private set; } = null;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Card;

        return true;
    }

    public void SetInfo(int templateId)
    {
        // Data
        CardData = Managers.Data.CardDic[templateId];
        CardNum = CardData.CardNUm;
        CardShape = CardData.CardShape;

        TeamColor = Managers.Game.PlayerTeamColor;
        TeamData = Managers.Data.TeamDic[(int)TeamColor];

        // Sorting
        SortingGroup sg = gameObject.GetOrAddComponent<SortingGroup>();
        sg.sortingLayerName = SortingLayers.Card_Layer_Name;
        sg.sortingOrder = SortingLayers.Card_Layer_Order;

        // Sprite Setting
        // _frontImage = Managers.Resource.Load<Sprite>(CardData.FrontSpriteName);
        BackImage = Managers.Resource.Load<Sprite>(TeamData.BackSpriteName);



        CardState = ECardState.Idle;
    }

    #region Animation
    protected override void UpdateAnimation()
    {
        switch (CardState)
        {
            case ECardState.Idle:
                // SpriteRenderer.sprite = _backImage;
                break;
        }
    }
    #endregion

    #region Update
    public override void UpdateController()
    {
        switch (CardState)
        {
            case ECardState.Idle:
                UpdateIdle();
                break;
        }
    }

    protected virtual void UpdateIdle()
    {

    }
    #endregion
}
