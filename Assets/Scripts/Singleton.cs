using UnityEngine;
using System;
using Random = System.Random;

public class Singleton : MonoBehaviour
{
    public const int MaxStage = 10;

    public static GameManager gm;

    //Scene Manager
    public static LobbySM lobbySM;
    public static SelectStageSM selectStageSM;
    public static StageSM stageSM;
    public static SurvivalSM survivalSM;

    //Player
    public static PlayerMovement player;

    //Value
    public static Random random = new Random();
    public static GameInfos gameInfos = new GameInfos();

    //Objects
    public static Pathfinding pathFinding;

    //Test
    public static SelectStageUISM selectStageUISM;
}
