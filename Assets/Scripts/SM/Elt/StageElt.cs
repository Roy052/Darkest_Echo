using UnityEngine;
using UnityEngine.UI;

public class StageElt : MonoBehaviour
{
    public Text textNumber;

    public void Set(int num)
    {
        textNumber.text = Extended.ConvertToRoman(num);
    }
}
