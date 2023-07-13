using UnityEngine;
using System;
using Random = System.Random;

public class Singleton : MonoBehaviour
{
    public static GameManager gm;
    public static LobbySM lobbySM;
    public static StageSM stageSM;

    public static Random random = new Random();
    public static GameInfos gameInfos = new GameInfos();
}
