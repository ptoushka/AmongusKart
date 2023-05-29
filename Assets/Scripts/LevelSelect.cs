using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public Button[] LevelButtons;

    void Awake()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i != 0)
            {
                if (PlayerPrefs.GetFloat(string.Concat(i - 1, "t")) <= 0) LevelButtons[i].interactable = true;
            }

            Debug.Log(string.Concat(i, " - ", PlayerPrefs.GetFloat(string.Concat(i, "t"), 0.0f), " - ", PlayerPrefs.GetInt(string.Concat(i, "s"), 0)));

            LevelHolder level = LevelButtons[i].GetComponent<LevelHolder>();
            level.SetTime(PlayerPrefs.GetFloat(string.Concat(i, "t"), 0.0f));
            level.SetStars(PlayerPrefs.GetInt(string.Concat(i, "s"), 0));
            PlayerPrefs.Save();
        }
    }
}