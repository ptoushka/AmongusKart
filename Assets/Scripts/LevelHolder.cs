using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelHolder : MonoBehaviour
{
    public TMP_Text TimeText;
    public StarHolder Stars;

    public void SetTime(float time)
    {
        int mills = (int)((time - Mathf.Floor(time)) * 100);
        string rem1 = ".";
        if (mills < 10) rem1 = ".0";

        int seconds = (int)(time % 60);
        string rem2 = ":";
        if (seconds < 10) rem2 = ":0";

        int minutes = (int)(((time - seconds) - (time - seconds) % 60) / 60);

        TimeText.SetText(string.Concat(minutes, rem2, seconds, rem1, mills));
    }

    public void SetStars(int am)
    {
        Stars.SetAmount(am);
    }
}
