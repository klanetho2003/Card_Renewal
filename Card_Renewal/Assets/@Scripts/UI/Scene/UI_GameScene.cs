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

    List<UI_GameScenePVP_Card> _cardUIList_PVP = new List<UI_GameScenePVP_Card>();
    List<UI_GameSceneMatch_Card> _cardUIList_MATCH = new List<UI_GameSceneMatch_Card>();
    const int MAX_ITEM_COUNT = 30;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        #region Bind
        // BindTexts(typeof(Texts));
        BindObjects(typeof(GameObjects));
        #endregion

        #region Event Bind
        // GetButton((int)Buttons.ItemsListButton).gameObject.BindEvent(OnClickItemsListButton);

        Managers.CardManager.OnChangePvpCardCount += Refresh;
        #endregion

        #region Instantiate Pre & Cache

        _cardUIList_PVP.Clear();
        var cardsParent_PVP = GetObject((int)GameObjects.CardsList_PVP);
        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            UI_GameScenePVP_Card card = Managers.UI.MakeSubItem<UI_GameScenePVP_Card>(cardsParent_PVP.transform);
            _cardUIList_PVP.Add(card);
            card.gameObject.SetActive(false);
        }

        _cardUIList_MATCH.Clear();
        var cardsParent_Match = GetObject((int)GameObjects.CardsList_MATCH);
        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            UI_GameSceneMatch_Card card = Managers.UI.MakeSubItem<UI_GameSceneMatch_Card>(cardsParent_Match.transform);
            _cardUIList_MATCH.Add(card);
            card.gameObject.SetActive(false);
        }

        #endregion

        Refresh();

        return true;
    }

    public void Setinfo()
    {
        Refresh();
    }

    void Refresh()
    {
        Refresh_PVPCards(_cardUIList_PVP, Managers.CardManager.PvpCards);
        Refresh_MatchCards(_cardUIList_MATCH, Managers.CardManager.MatchCards);
    }

    void Refresh_PVPCards(List<UI_GameScenePVP_Card> uiCardList, List<PvpCard> CardList)
    {
        for (int i = 0; i < uiCardList.Count; i++)
        {
            if (i < CardList.Count)
            {
                PvpCard card = CardList[i];
                var cardUI = uiCardList[i];

                // PvpCard UI SetInfo
                cardUI.SetInfo(card);
            }
            else
            {
                uiCardList[i].gameObject.SetActive(false);
            }
        }
    }

    void Refresh_MatchCards(List<UI_GameSceneMatch_Card> uiCardList, List<MatchCard> CardList)
    {
        for (int i = 0; i < uiCardList.Count; i++)
        {
            if (i < CardList.Count)
            {
                MatchCard card = CardList[i];
                var cardUI = uiCardList[i];

                // MatchCard UI SetInfo
                cardUI.SetInfo(card);
            }
            else
            {
                uiCardList[i].gameObject.SetActive(false);
            }
        }
    }

    private float _elapsedTime = 0.0f;
    private float _updateInterval = 1.0f;
    private void Update()
    {
        #region FPS Viewer
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _updateInterval)
        {
            float fps = 1.0f / Time.deltaTime;
            float ms = Time.deltaTime * 1000.0f;
            string text = string.Format("{0:N1} FPS ({1:N1}ms)", fps, ms);
            // _cacheFPSViewerText.text = text;

            _elapsedTime = 0;
        }
        #endregion
    }
}
