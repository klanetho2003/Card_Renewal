using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Mathematics;
using UnityEngine;
using static Define;

namespace Data
{
    #region Example
    public class ParentData
    {
        public int TemplateId;
    }

    [Serializable]
    public class ChildData : ParentData
    {
        public int TempIntData;
        public string TempStringData;
        public List<ListData> TempListData = new List<ListData>();
    }

    [Serializable]
    public class ListData
    {
        public int Value_1;
        public int ObjectTemplateId;
    }

    [Serializable]
    public class ChildDataLoader : ILoader<int, ChildData>
    {
        public List<ChildData> Childs = new List<ChildData>();
        public Dictionary<int, ChildData> MakeDict()
        {
            Dictionary<int, ChildData> dict = new Dictionary<int, ChildData>();
            foreach (ChildData child in Childs)
                dict.Add(child.TemplateId, child);
            return dict;
        }
    }
    #endregion

    #region Card
    [Serializable]
    public class CardData
    {
        public int TemplateId;
        public ECardShape CardShape;
        public ECardNum CardNUm;

        public string FrontSpriteName;
    }

    [Serializable]
    public class CardDataLoader : ILoader<int, CardData>
    {
        public List<CardData> cards = new List<CardData>();
        public Dictionary<int, CardData> MakeDict()
        {
            Dictionary<int, CardData> dict = new Dictionary<int, CardData>();
            foreach (CardData card in cards)
                dict.Add(card.TemplateId, card);
            return dict;
        }
    }
    #endregion

    #region Team
    [Serializable]
    public class TeamData
    {
        public int ETeamColor;

        public string BackSpriteName;
    }

    [Serializable]
    public class TeamDataLoader : ILoader<int, TeamData>
    {
        public List<TeamData> teams = new List<TeamData>();
        public Dictionary<int, TeamData> MakeDict()
        {
            Dictionary<int, TeamData> dict = new Dictionary<int, TeamData>();
            foreach (TeamData team in teams)
                dict.Add(team.ETeamColor, team);
            return dict;
        }
    }
    #endregion
}
