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
    [SerializeField] private TextMeshProUGUI BuyEquipText;
    [SerializeField] private TextMeshProUGUI CoinsText;

    private void Start()
    {
        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        UpdateBuyEquipText();
        equipedSkin = PlayerPrefs.GetInt("EquipedSkin");
        gm.skin = skins[equipedSkin].skin;
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

    public void EquipSkin()
    {
        equipedSkin = currentlyDisplayedSkin;
        gm.skin = skins[currentlyDisplayedSkin].skin;
        PlayerPrefs.SetInt("EquipedSkin", equipedSkin);
        UpdateBuyEquipText();
    }

    public void UpdateBuyEquipText()
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

    public void UpdateCoinsText()
    {
        CoinsText.text = gm.GetCoins().ToString();
    }
}