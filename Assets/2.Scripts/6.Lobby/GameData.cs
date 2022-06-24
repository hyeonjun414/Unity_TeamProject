﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePlayerInput
{
    NULL,
    MOVE_UP,
    MOVE_RIGHT,
    MOVE_DOWN,
    MOVE_LEFT,
    ROTATE_RIGHT,
    ROTATE_LEFT,
    ATTACK,
    BLOCK,
    USE_ITEM,
    CHANGE_ITEM_SLOT,
}

public enum PlayerDir
{
    Start,
    Up,
    Right,
    Down,
    Left,
    End
}

public enum PlayerState
{
    Normal,
    Move,
    Defend,
    Attack,
    Stun,
    Dead
}

public enum ModeType
{
    LastFighter,
    OneShot,
    TimeToKill,
    End,
}

public enum MapType
{
    Normal,
    Forest,
    Snow,
    End
}

public class GameData : MonoBehaviour
{
    public const int COUNTDOWN = 0;

    public const string PLAYER_READY = "Ready";
    public const string PLAYER_LOAD = "Load";
    public const string PLAYER_GEN = "Character";

    public const string PLAYER_INDEX = "PlayerIndex";

    public const string GAME_MODE = "GameMode";
    public const string GAME_MAP = "GameMap";
  
    public const string IS_EMAIL = "IsEmail";

    [Header("게임결과정보 저장")]
    public const string PLAYER_NAME = "PlayerName";
    public const string PLAYER_KILL = "PlayerKill";
    public const string PLAYER_DEAD = "PlayerDead";
    public const string PLAYER_RANK = "PlayerRank";s
    public static string GetMode(ModeType type)
    {
        switch (type)
        {
            case ModeType.LastFighter:
                return "Last Fighter";
            case ModeType.OneShot:
                return "One Shot";
            case ModeType.TimeToKill:
                return "Time To Kill";
            default: return "";
        }

    }
    public static string GetMap(MapType type)
    {
        switch (type)
        {
            case MapType.Normal:
                return "Normal";
            case MapType.Forest:
                return "Forest";
            case MapType.Snow:
                return "Snow";
            default: return "";
        }
    }

    
    public static Color GetColor(int playerNumber)
    {
        switch(playerNumber)
        {
            case 0: return Color.red;
            case 1: return Color.green;
            case 2: return Color.blue;
            case 3: return Color.yellow;
            case 4: return Color.cyan;
            case 5: return Color.magenta;
            case 6: return Color.white;
            case 7: return Color.black;
            default: return Color.grey;
        }
    }
}
