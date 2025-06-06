using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

[Serializable]
public class GameSaveData
{
    //Temp - Value
    public int Temp = 0;
    // ..

    // List
    public List<CardSaveData> Cards = new List<CardSaveData>(); // Child를 Item으로 가정

    public int DbIdGenerator = 1;
}

[Serializable]
public class CardSaveData
{
    public int TemplateDataId = 0;
    public int SaveValue_1 = 1;
    public int SaveValue_2 = 0;
    public TempOwningState OwningState = TempOwningState.Lock;
}

public enum TempOwningState
{
    Lock,       // 발견 X
    Unowned,    // 발견 O, 보유 X
    Owned,      // 보유
    Picked,     // 장비 중
}

public class GameManager
{
    public ETeamColor PlayerTeamColor { get; set; } = ETeamColor.Red;

    #region Data - Init, Save, Load

    // 진행에 따라 달라지는 Data
    GameSaveData _saveData = new GameSaveData();
    public GameSaveData SaveData { get { return _saveData; } set { _saveData = value; } }

    public int GenerateItemDbId()
    {
        int itemDbId = _saveData.DbIdGenerator;
        _saveData.DbIdGenerator++;
        return itemDbId;
    }

    public string Path { get { return Application.persistentDataPath + "/SaveData.json"; } }

    public void InitGame()
    {
        if (File.Exists(Path))
            return;

        // To Do : 패치에 Data들을 대처할 수 있도록 수정 -> Version 정보를 참조
        // var cards = Managers.Data.CardDic.Values.ToList();
        // foreach (CardData card in cards)
        // {
        //     CardSaveData cardSaveData = new CardSaveData()
        //     {
        //         TemplateDataId = card.TemplateId,
        //     };
        // 
        //     SaveData.Cards.Add(cardSaveData);
        // }
    }

    public void SaveGame()
    {
        // Example
        {
            // Example
            /*SaveData.Children.Clear();
            foreach (var item in Managers.Inventory.AllItems)
                SaveData.Children.Add(item.SaveData);*/
        }

        string jsonStr = JsonUtility.ToJson(Managers.Game.SaveData);
        File.WriteAllText(Path, jsonStr);
        Debug.Log($"Save Game Completed : {Path}");
    }

    public bool LoadGame()
    {
        if (File.Exists(Path) == false)
            return false;

        string fileStr = File.ReadAllText(Path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(fileStr);

        if (data != null)
            Managers.Game.SaveData = data;

        // Example
        {
            /*Managers.Inventory.Clear();
            foreach (ChildSaveData itemSaveData in data.Children)
                Managers.Inventory.AddItem(itemSaveData);*/
        }

        Debug.Log($"Save Game Loaded : {Path}");
        return true;
    }
    #endregion
}
