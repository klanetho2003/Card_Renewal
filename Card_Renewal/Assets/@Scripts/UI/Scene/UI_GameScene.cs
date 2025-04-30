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

    List<UI_GameScenePVP_Card> _cardUIList_PVP = new List<UI_GameScenePVP_Card>();
    List<UI_GameSceneMatch_Card> _cardUIList_MATCH = new List<UI_GameSceneMatch_Card>();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Bind
        // BindTexts(typeof(Texts));
        BindObjects(typeof(GameObjects));

        // Event Bind
        // GetButton((int)Buttons.AddCardButton).gameObject.BindEvent(OnClickAddCardButton);
        Managers.CardManager.OnChangePvpCardCount += Refresh;

        // Instantiate Pre & Cache
        InitializeCardUI<UI_GameScenePVP_Card, PvpCard>(_cardUIList_PVP, GetObject((int)GameObjects.CardsList_PVP).transform);
        InitializeCardUI<UI_GameSceneMatch_Card, MatchCard>(_cardUIList_MATCH, GetObject((int)GameObjects.CardsList_MATCH).transform);

        Refresh();

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

    private void InitializeCardUI<TUI, TCard>(List<TUI> uiList, Transform parent) where TUI : UI_GameScene_CardBase<TCard> where TCard : CardBase
    {
        uiList.Clear();
        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            var cardUI = Managers.UI.MakeSubItem<TUI>(parent);
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
