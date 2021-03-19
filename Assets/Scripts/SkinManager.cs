using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : BaseManager<SkinManager>
{
    public Color btnChoosen;
    public Color btnUnchoosen;
    public Color[] playerSkinColors;
    public Color[] playerSkinEmmisionColors;
    public Skin[] playerSkins;
    public Skin[] swordSkins;
    public Skin[] playerExampleSkins;
    public Skin[] swordExampleSkins;
    public Skin currentPlayerSkin; 
    public Skin currentPlayerExampleSkin; 
    public Skin currentSwordSkin; 
    public Skin currentSwordExampleSkin; 
    public GameObject currentShop;
    public Image currentBtnImage;
    public SkinnedMeshRenderer playerSkinMesh;
    public SkinnedMeshRenderer playerExampleSkinMesh;
    
    protected override void Awake()
    {
        if (!instance)
            instance = this;
        base.Awake();
    }

    protected override void InitializeManager()
    {
        ChoosePlayerSkin(true);
        ChooseSwordSkin(true);
    }

    public void SetPlayerSkin(int num)
    {
        currentPlayerSkin.ActiveSkin(false);
        ChoosePlayerSkin(false);
        PlayerPrefs.SetInt("PlayerSkin", num); 
    }

    public void SetSwordSkin(int num)
    {
        currentSwordSkin.ActiveSkin(false);
        ChooseSwordSkin(false);
        PlayerPrefs.SetInt("SwordSkin", num);
    }

    public void ChoosePlayerSkin(bool isChoosen)
    {
        int playerSkin = PlayerPrefs.GetInt("PlayerSkin", 0);
        //playerShopBtns[playerSkin].interactable = !isChoosen;
        playerSkins[playerSkin].ActiveSkin(isChoosen);
        currentPlayerSkin = playerSkins[playerSkin];
        playerExampleSkins[playerSkin].ActiveSkin(isChoosen);
        currentPlayerExampleSkin = playerExampleSkins[playerSkin];
        playerExampleSkinMesh.material.color = playerSkinMesh.material.color = playerSkinColors[playerSkin];
        playerExampleSkinMesh.material.SetColor("_EmissionColor", playerSkinEmmisionColors[playerSkin]);
        playerSkinMesh.material.SetColor("_EmissionColor", playerSkinEmmisionColors[playerSkin]);
    }
    
    public void ChooseSwordSkin(bool isChoosen)
    {
        int swordSkin = PlayerPrefs.GetInt("SwordSkin", 0);
        //swordShopBtns[swordSkin].interactable = !isChoosen;
        swordSkins[swordSkin].ActiveSkin(isChoosen);
        currentSwordSkin = swordSkins[swordSkin];
        swordExampleSkins[swordSkin].ActiveSkin(isChoosen);
        currentSwordExampleSkin = swordExampleSkins[swordSkin];
    }

    public void ChooseShop(Shop shop)
    {
        currentShop.SetActive(false);
        currentBtnImage.color = btnUnchoosen;
        shop.gameObject.SetActive(true);
        shop.shopBtnImage.color = btnChoosen;
        currentBtnImage = shop.shopBtnImage;
        currentShop = shop.gameObject;
    }
}
