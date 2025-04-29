using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = EScene.GameScene;

        // To Do
        SetPvpCards(3, TempCard_Heart_Q);
        SetPvpCards(4, TempCard_Spade_Q);

        SetMatchCards(2, TempCard_Heart_Q);
        SetMatchCards(5, TempCard_Spade_Q);

        UI_GameScene gameScene = Managers.UI.ShowSceneUI<UI_GameScene>();
        gameScene.Setinfo();

        return true;
    }

    void SetPvpCards(int cardsCount, int templateId)
    {
        for (int i = 0; i < cardsCount; i++)
            Managers.CardManager.AddPvpCard(templateId);
    }

    void SetMatchCards(int cardsCount, int templateId)
    {
        for (int i = 0; i < cardsCount; i++)
            Managers.CardManager.AddMatchCard(templateId);
    }

    public override void Clear()
    {
        
    }
}
