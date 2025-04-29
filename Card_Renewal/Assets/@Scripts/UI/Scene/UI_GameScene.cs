using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;
using static UnityEditor.Progress;

public class UI_GameScene : UI_Scene
{
    #region Enum to Bind
    enum GameObjects
    {
        CardsList,
    }
    enum Texts
    {
        
    }
    #endregion

    List<UI_GameScene_Card> _cardUIList = new List<UI_GameScene_Card>();
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

        Managers.CardManager.OnChangeCardCount += Refresh;
        #endregion

        #region Instantiate Pre & Cache

        _cardUIList.Clear();
        var cardsParent = GetObject((int)GameObjects.CardsList);
        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            UI_GameScene_Card card = Managers.UI.MakeSubItem<UI_GameScene_Card>(cardsParent.transform);
            _cardUIList.Add(card);
            card.gameObject.SetActive(false);
        }

        #endregion

        return true;
    }

    public void Setinfo()
    {
        Refresh();
    }

    void Refresh()
    {
        Refresh_Item(_cardUIList, Managers.CardManager.Cards);
    }

    void Refresh_Item(List<UI_GameScene_Card> uiCardList, List<Card> CardList)
    {
        for (int i = 0; i < uiCardList.Count; i++)
        {
            if (i < CardList.Count)
            {
                Card card = CardList[i];
                var cardUI = uiCardList[i];

                // Card UI SetInfo
                cardUI.SetInfo(card);
                cardUI.gameObject.SetActive(true);
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
