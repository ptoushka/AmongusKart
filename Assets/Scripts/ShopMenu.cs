using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using static UnityEngine.InputManagerEntry;

public class ShopMenu : MonoBehaviour
{
    public TMP_Text BalanceText;
    public Button[] SkinButtons;
    public int[] SkinPrices;
    public Material[] PlayerSkins;
    public Renderer Model;

    public void SetSkin(int skinID)
    {
        PlayerPrefs.SetInt("SelSkin", skinID);
        PlayerPrefs.Save();
        for (int i = 0; i < SkinButtons.Length; i++)
        {
            if (PlayerPrefs.GetInt(string.Concat("Skin", i), 0) == 0) continue;
            if (i == skinID)
            {
                SkinButtons[i].transform.Find("Aval").GetComponent<TMP_Text>().text = "•";
            }
            else
            {
                SkinButtons[i].transform.Find("Aval").GetComponent<TMP_Text>().text = string.Empty;
            }
        }
        Model.material = PlayerSkins[skinID];
    }

    public void Buy(int skinID)
    {
        if (PlayerPrefs.GetInt("Money", 0) >= SkinPrices[skinID])
        {
            PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0) - SkinPrices[skinID]);
            PlayerPrefs.SetInt(string.Concat("Skin", skinID), 1);
            SkinButtons[skinID].interactable = true;
            SkinButtons[skinID].transform.Find("Aval").GetComponent<TMP_Text>().text = "";
            SkinButtons[skinID].transform.Find("Buy").gameObject.SetActive(false);
            PlayerPrefs.Save();
        }
    }

    void Awake()
    {
        PlayerPrefs.SetInt("Skin0", 1);
        PlayerPrefs.Save();
        SetSkin(PlayerPrefs.GetInt("SelSkin", 0));
        BalanceText.text = PlayerPrefs.GetInt("Money", 0).ToString();
        for(int i = 1; i < SkinButtons.Length; i++)
        {
            if (PlayerPrefs.GetInt(string.Concat("Skin", i), 0) == 1)
            {
                SkinButtons[i].interactable = true;
                SkinButtons[i].transform.Find("Aval").GetComponent<TMP_Text>().text = "";
                SkinButtons[i].transform.Find("Buy").gameObject.SetActive(false);
            }
        }

        var skinID = PlayerPrefs.GetInt("SelSkin", 0);
        SetSkin(skinID);
    }
}
