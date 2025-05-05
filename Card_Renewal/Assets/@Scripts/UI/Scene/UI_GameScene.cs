using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_GameScene : UI_Scene
{
    #region Enum to Bind
    enum GameObjects
    {
        CardsList_PVP,
        CardsList_MATCH,
    }
    enum Texts
    {
        
    }
    #endregion

    const int MAX_ITEM_COUNT = 30;

    private List<UI_GameScenePVP_Card> _cardUIList_PVP = new List<UI_GameScenePVP_Card>();
    private List<UI_GameSceneMatch_Card> _cardUIList_MATCH = new List<UI_GameSceneMatch_Card>();

    private GameObject _pvpCardLIstParent = null;
    private GameObject _matchCardLIstParent = null;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Bind
        BindObjects(typeof(GameObjects));

        // Event Bind
        Managers.CardManager.OnChangePvpCardCount += Refresh;

        // Cache
        _pvpCardLIstParent = GetObject((int)GameObjects.CardsList_PVP);
        _matchCardLIstParent = GetObject((int)GameObjects.CardsList_MATCH);

        // Set Card Size
        {
            _pvpCardLIstParent.GetComponent<GridLayoutGroup>().cellSize 
                = new Vector2(
                    Managers.CardManager.InitGameData.PVPCardSizeX,
                    Managers.CardManager.InitGameData.PVPCardSizeY);
            _matchCardLIstParent.GetComponent<GridLayoutGroup>().cellSize
                = new Vector2(
                Managers.CardManager.InitGameData.MatchCardSizeX,
                Managers.CardManager.InitGameData.MatchCardSizeY);
        }

        // Instantiate Pre & Cache
        InitializeCardUI<UI_GameScenePVP_Card, PvpCard>(_cardUIList_PVP, _pvpCardLIstParent.transform);
        InitializeCardUI<UI_GameSceneMatch_Card, MatchCard>(_cardUIList_MATCH, _matchCardLIstParent.transform);

        return true;
    }

    public void Setinfo()
    {
        Refresh();
    }

    void Refresh()
    {
        RefreshCards(_cardUIList_PVP, Managers.CardManager.PvpCards);
        RefreshCards(_cardUIList_MATCH, Managers.CardManager.MatchCards);
    }

    private void InitializeCardUI<TUI, TCard>(List<TUI> uiList, Transform parent, string profabName = "UI_GameScene_CardPrefab") where TUI : UI_GameScene_CardBase<TCard> where TCard : CardBase
    {
        uiList.Clear();
        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            var cardUI = Managers.UI.MakeSubItem<TUI>(parent: parent, name: profabName);
            cardUI.gameObject.SetActive(false);
            uiList.Add(cardUI);
        }
    }

    private void RefreshCards<TUI, TCard>(List<TUI> uiCardList, List<TCard> cardList) where TUI : UI_GameScene_CardBase<TCard> where TCard : CardBase
    {
        for (int i = 0; i < uiCardList.Count; i++)
        {
            if (i < cardList.Count)
            {
                uiCardList[i].SetInfo(cardList[i]);
            }
            else
            {
                uiCardList[i].gameObject.SetActive(false);
            }
        }
    }
}
