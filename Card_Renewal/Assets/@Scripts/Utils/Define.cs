using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    #region Manager
    public enum EScene
    {
        Unknown,
        TitleScene,
        GameScene,
    }

    public enum KeyDownEvent
    {
        Space = 100,
    }

    public enum Sound
    {
        Bgm,
        Effect,
    }

    public enum UIEvent
    {
        None,

        PointerEnter,
        PointerExit,
        Click,

        Pressed,
        PointerDown,
        PointerUp,

        BeginDrag,
        Drag,
        EndDrag,
    }
    #endregion

    public enum ECardNum
    {
        None,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        J = 11,
        Q = 12,
        K = 13,

        Joker = 20
    }

    public enum ECardShape
    {
        None,
        Club = 12,
        Diamond = 34,
        Heart = 56,
        Spade = 78,
    }

    public enum ETeamColor
    {
        None,
        Red = 1,
    }

    public enum ECardState
    {
        None,
        Idle,
        Hover,
        PointDown,
        PointUp,
        Select,
        Moving,
    }

    public enum EObjectType
    {
        None,
        Card,
    }

    public enum ELayer
    {
        Default = 0,
        TransparentFX = 1,
        IgnoreRaycast = 2,
        Dummy1 = 3,
        Water = 4,
        UI = 5,
    }

    // Hard
    public const int TempCard_Heart_Q = 62;
    public const int TempCard_Spade_Q = 82;

    public static class SortingLayers
    {
        public const int Card_Layer_Order = 300;
        public const string Card_Layer_Name = "Card";
    }
}
