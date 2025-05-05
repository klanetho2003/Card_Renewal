using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    // Cache
    private CardManager _cardManager { get { return Managers.CardManager; } }
    private InitGameData _initGameData { get { return Managers.CardManager.InitGameData; } }

    public override bool Init()
    {
        if (!base.Init())
            return false;

        SceneType = EScene.GameScene;

        InitializeCards();
        InitializeUI();

        return true;
    }

    // To Do : InitGame Data Parsing
    private void InitializeCards()
    {
        AddCards(_cardManager.AddPvpCard, _cardManager.GetPVPCardRandomId, _initGameData.PVPCardCount);
        AddCards(_cardManager.AddMatchCard, _cardManager.GetMatchCardRandomId, _initGameData.MatchCardCount);
    }

    private void AddCards(Action<int> addCardFunc, Func<int> getIdFunc, int count)
    {
        for (int i = 0; i < count; i++)
        {
            int templateId = getIdFunc.Invoke();
            addCardFunc.Invoke(templateId);
        }
    }

    private void InitializeUI()
    {
        UI_GameScene gameScene = Managers.UI.ShowSceneUI<UI_GameScene>();
        gameScene.Setinfo();
    }

    public override void Clear()
    {
        
    }
}
