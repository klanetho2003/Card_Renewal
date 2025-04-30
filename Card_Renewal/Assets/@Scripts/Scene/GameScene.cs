using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
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
        AddCards(Managers.CardManager.AddPvpCard, 3, TempCard_Heart_Q);
        AddCards(Managers.CardManager.AddPvpCard, 4, TempCard_Spade_Q);

        AddCards(Managers.CardManager.AddMatchCard, 2, TempCard_Heart_Q);
        AddCards(Managers.CardManager.AddMatchCard, 5, TempCard_Spade_Q);
    }

    private void AddCards(System.Action<int> addCardFunc, int count, int templateId)
    {
        for (int i = 0; i < count; i++)
            addCardFunc.Invoke(templateId);
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
