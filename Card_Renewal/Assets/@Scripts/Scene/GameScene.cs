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
        UI_GameScene gameScene = Managers.UI.ShowSceneUI<UI_GameScene>();

        SetCards(5);

        gameScene.Setinfo();

        return true;
    }

    void SetCards(int cardsCount)
    {
        for (int i = 0; i < cardsCount; i++)
        {
            Managers.CardManager.AddCard(TempCard_Heart_Q);
        }
    }

    public override void Clear()
    {
        
    }
}
