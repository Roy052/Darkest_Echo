using UnityEngine;
using UnityEngine.UI;

public class StageElt : MonoBehaviour
{
    public Text textNumber;
    int num = 0;

    public void Set(int num)
    {
        textNumber.text = Extended.ConvertToRoman(num);
        this.num = num;
    }

    public void OnTouch()
    {
        Singleton.gm.stageNum = num;
        Singleton.gm.LoadStage(num);
    }
}
