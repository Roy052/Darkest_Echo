using System.Collections;
using System.Collections.Generic;

public class GameInfos : Singleton
{
    public Dictionary<int, string> stageTitle = new Dictionary<int, string>()
    {
        {1, "첫 발자국"},
        {2, "늦은 출발" },
        {3, "굶주림" },
        {4, "먹이" },
        {5, "늦은 식사" },
        {6, "" }
    };
}
