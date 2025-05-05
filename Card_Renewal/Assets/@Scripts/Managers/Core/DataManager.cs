using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.InitGameData> InitGameDic { get; private set; } = new Dictionary<int, Data.InitGameData>();
    public Dictionary<int, Data.CardData> CardDic { get; private set; } = new Dictionary<int, Data.CardData>();
    public Dictionary<int, Data.TeamData> TeamDic { get; private set; } = new Dictionary<int, Data.TeamData>();

    public void Init()
    {
        InitGameDic = LoadJson<Data.InitGameDataLoader, int, Data.InitGameData>("InitGameData").MakeDict();
        CardDic = LoadJson<Data.CardDataLoader, int, Data.CardData>("CardData").MakeDict();
        TeamDic = LoadJson<Data.TeamDataLoader, int, Data.TeamData>("TeamData").MakeDict();
    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}
