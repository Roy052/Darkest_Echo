using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extended
{
    public static string ConvertToRoman(int num)
    {
        if (num < 1 || num > 3999)
            Debug.LogError("Roman numerals is 1~3999");

        Dictionary<int, string> romanNumerals = new Dictionary<int, string>
        {
            {1000, "M"},
            {900, "CM"},
            {500, "D"},
            {400, "CD"},
            {100, "C"},
            {90, "XC"},
            {50, "L"},
            {40, "XL"},
            {10, "X"},
            {9, "IX"},
            {5, "V"},
            {4, "IV"},
            {1, "I"}
        };

        string roman = string.Empty;

        foreach (var kvp in romanNumerals)
        {
            while (num >= kvp.Key)
            {
                roman += kvp.Value;
                num -= kvp.Key;
            }
        }

        return roman;
    }
}
