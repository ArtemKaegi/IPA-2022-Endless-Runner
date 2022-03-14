using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SkinsController : MonoBehaviour
{
    [SerializeField] private SkinObjectController[] skins;
    private int equipedSkin;
    private int currentlyDisplayedSkin = 0;
    private GameManager gm;
    private bool[] bought;
    [SerializeField] private TextMeshProUGUI BuyEquipText;
    [SerializeField] private TextMeshProUGUI CoinsText;

    private void Start()
    {
        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        equipedSkin = PlayerPrefs.GetInt("EquipedSkin");
        gm.skin = skins[equipedSkin].skin;
        bought = new bool[skins.Length];
        for (int i = 0; i < bought.Length; i++)
        {
            bought[i] = PlayerPrefs.GetString("Skin" + i) == "Bought";
        }

        bought[0] = true;
        Debug.Log(bought[1]);
        UpdateBuyEquipText();
    }

    public void NextSkin()
    {
        if (currentlyDisplayedSkin < skins.Length - 1)
        {
            skins[currentlyDisplayedSkin].gameObject.SetActive(false);
            currentlyDisplayedSkin += 1;
            skins[currentlyDisplayedSkin].gameObject.SetActive(true);
            UpdateBuyEquipText();
        }
    }

    public void PreviousSkin()
    {
        if (currentlyDisplayedSkin > 0)
        {
            skins[currentlyDisplayedSkin].gameObject.SetActive(false);
            currentlyDisplayedSkin -= 1;
            skins[currentlyDisplayedSkin].gameObject.SetActive(true);
            UpdateBuyEquipText();
        }
    }

    public void CheckButton()
    {
        Debug.Log(bought[currentlyDisplayedSkin]);
        if (bought[currentlyDisplayedSkin])
        {
            if (currentlyDisplayedSkin != equipedSkin)
            {
                EquipSkin();
            }
        }
        else
        {
            if (gm.GetCoins() > skins[currentlyDisplayedSkin].price)
            {
                gm.RemoveCoins(skins[currentlyDisplayedSkin].price);
                PlayerPrefs.SetString("Skin" + currentlyDisplayedSkin, "Bought");
                bought[currentlyDisplayedSkin] = true;
                UpdateBuyEquipText();
                UpdateCoinsText();
            }
        }
    }

    private void EquipSkin()
    {
        equipedSkin = currentlyDisplayedSkin;
        gm.skin = skins[currentlyDisplayedSkin].skin;
        PlayerPrefs.SetInt("EquipedSkin", equipedSkin);
        UpdateBuyEquipText();
    }

    public void UpdateBuyEquipText()
    {
        if (bought[currentlyDisplayedSkin])
        {
            if (currentlyDisplayedSkin == equipedSkin)
            {
                BuyEquipText.text = "Equiped";
            }
            else
            {
                BuyEquipText.text = "Equip";
            }
        }
        else
        {
            BuyEquipText.text = "Buy " + skins[currentlyDisplayedSkin].price;
        }
    }

    public void UpdateCoinsText()
    {
        CoinsText.text = gm.GetCoins().ToString();
    }
}