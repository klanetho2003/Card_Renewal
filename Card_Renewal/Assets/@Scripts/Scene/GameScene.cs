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
        SetCards(2, TempCard_Heart_Q);
        SetCards(3, TempCard_Spade_Q);

        UI_GameScene gameScene = Managers.UI.ShowSceneUI<UI_GameScene>();
        gameScene.Setinfo();

        return true;
    }

    void SetCards(int cardsCount, int templateId)
    {
        for (int i = 0; i < cardsCount; i++)
        {
            Managers.CardManager.AddCard(templateId);
        }
    }

    public override void Clear()
    {
        
    }
}
