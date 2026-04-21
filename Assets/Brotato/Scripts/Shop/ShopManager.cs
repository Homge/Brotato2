using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

using Random = UnityEngine.Random;

public class ShopManager : MonoBehaviour, IGameStateListener
{
    [Header(" Elements ")]
    [SerializeField] private Transform containersParent;
    [SerializeField] private ShopItemContainer shopItemContainerPrefab;

    [Header(" Player Components ")]
    [SerializeField] private PlayerWeapons playerWeapons;
    [SerializeField] private PlayerObjects playerObjects;

    [Header(" Reroll ")]
    [SerializeField] private Button rerollButton;
    [SerializeField] private TextMeshProUGUI rerollPriceText;

    [Header(" Reroll Balance Settings ")]
    [SerializeField] private int baseRerollPrice = 2; 
    [SerializeField] private int pricePerRerollIncrement = 5; 
    private int timesRerolledInCurrentShop = 0;

    [Header(" Actions ")]
    public static Action onItemPurchased;

    private void Awake()
    {
        ShopItemContainer.onPurchased += ItemPurchasedCallback;
        CurrencyManager.onUpdated    += CurrencyUpdatedCallback;
    }

    private void OnDestroy()
    {
        ShopItemContainer.onPurchased -= ItemPurchasedCallback;
        CurrencyManager.onUpdated    -= CurrencyUpdatedCallback;
    }

    void Start()  { }
    void Update() { }

    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState == GameState.SHOP)
        {
            timesRerolledInCurrentShop = 0;
            Configure();
            UpdateRerollVisuals();
        }
    }

    private int CalculateCurrentRerollPrice()
    {
        int waveIndex = WaveManager.instance != null ? WaveManager.instance.CurrentWaveIndex : 0;
        return (waveIndex + 1) * baseRerollPrice + (timesRerolledInCurrentShop * pricePerRerollIncrement);
    }

    private void Configure()
    {
        List<GameObject> toDestroy = new List<GameObject>();
        for (int i = 0; i < containersParent.childCount; i++)
        {
            ShopItemContainer container = containersParent.GetChild(i).GetComponent<ShopItemContainer>();
            if (!container.IsLocked)
                toDestroy.Add(container.gameObject);
        }
        while (toDestroy.Count > 0)
        {
            Transform t = toDestroy[0].transform;
            t.SetParent(null);
            Destroy(t.gameObject);
            toDestroy.RemoveAt(0);
        }

        int totalSlots  = 4;
        int toAdd       = totalSlots - containersParent.childCount;
        if (toAdd <= 0) return;

        int weaponCount = Mathf.Min(2, toAdd);
        int objectCount = toAdd - weaponCount;

        for (int i = 0; i < weaponCount; i++)
        {
            ShopItemContainer c = Instantiate(shopItemContainerPrefab, containersParent);
            c.Configure(ResourcesManager.GetRandomWeapon(), GetWeightedWeaponLevel());
        }

        for (int i = 0; i < objectCount; i++)
        {
            ShopItemContainer c = Instantiate(shopItemContainerPrefab, containersParent);
            c.Configure(GetWeightedRandomObject());
        }
    }

    private float GetPlayerLuck()
    {
        PlayerStatsManager psm = FindFirstObjectByType<PlayerStatsManager>();
        return psm != null ? psm.GetStatValue(Stat.Luck) : 0f;
    }

    private int GetWeightedWeaponLevel()
    {
        int  wave    = WaveManager.instance != null ? WaveManager.instance.CurrentWaveIndex : 0;
        bool endless = WaveManager.instance != null && WaveManager.instance.IsEndlessMode;
        return RollWeightedLevel(wave, endless, GetPlayerLuck());
    }

    private ObjectDataSO GetWeightedRandomObject()
    {
        int  wave    = WaveManager.instance != null ? WaveManager.instance.CurrentWaveIndex : 0;
        bool endless = WaveManager.instance != null && WaveManager.instance.IsEndlessMode;

        int targetRarity = RollWeightedLevel(wave, endless, GetPlayerLuck());
        ObjectDataSO[] all      = ResourcesManager.Objects;
        ObjectDataSO[] filtered = Array.FindAll(all, o => o.Rarity == targetRarity);
        return filtered.Length > 0
            ? filtered[Random.Range(0, filtered.Length)]
            : all[Random.Range(0, all.Length)];
    }

    private int RollWeightedLevel(int waveIndex, bool isEndless, float luck)
    {
        float[] weights;
        if (isEndless)
            weights = new float[] { 15f, 30f, 35f, 20f };
        else if (waveIndex <= 1) 
            weights = new float[] { 95f, 5f, 0f, 0f };
        else if (waveIndex < 5) 
            weights = new float[] { 75f, 25f, 0f, 0f };
        else if (waveIndex < 10) 
            weights = new float[] { 50f, 35f, 15f, 0f };
        else 
            weights = new float[] { 25f, 35f, 28f, 12f };

        float shift = Mathf.Clamp(luck / 10f * 3f, 0f, 30f);
        float actualShift = Mathf.Min(shift, weights[0] * 0.8f);
        weights[0] -= actualShift;

        if (waveIndex < 5)
        {
            weights[1] += actualShift;
        }
        else if (waveIndex < 10)
        {
            weights[1] += actualShift * 0.6f;
            weights[2] += actualShift * 0.4f;
        }
        else
        {
            weights[1] += actualShift * 0.4f;
            weights[2] += actualShift * 0.4f;
            weights[3] += actualShift * 0.2f;
        }

        float total = 0f;
        foreach (float w in weights) total += w;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative) return i;
        }

        return 0;
    }

    public void Reroll()
    {
        int currentPrice = CalculateCurrentRerollPrice();
        if (CurrencyManager.instance.HasEnoughCurrency(currentPrice))
        {
            CurrencyManager.instance.UseCurrency(currentPrice);
            timesRerolledInCurrentShop++;
            Configure();
            UpdateRerollVisuals();
        }
    }

    private void UpdateRerollVisuals()
    {
        int currentPrice = CalculateCurrentRerollPrice();
        rerollPriceText.text = currentPrice.ToString();
        rerollButton.interactable = CurrencyManager.instance.HasEnoughCurrency(currentPrice);
    }

    private void CurrencyUpdatedCallback() => UpdateRerollVisuals();

    private void ItemPurchasedCallback(ShopItemContainer container, int weaponLevel)
    {
        if (container.WeaponData != null)
            TryPurchaseWeapon(container, weaponLevel);
        else
            PurchaseObject(container);
    }

    private void TryPurchaseWeapon(ShopItemContainer container, int weaponLevel)
    {
        if (playerWeapons.TryAddWeapon(container.WeaponData, weaponLevel))
        {
            int price = WeaponStatsCalculator.GetPurchasePrice(container.WeaponData, weaponLevel);
            CurrencyManager.instance.UseCurrency(price);
            Destroy(container.gameObject);
        }

        onItemPurchased?.Invoke();
    }

    private void PurchaseObject(ShopItemContainer container)
    {
        playerObjects.AddObject(container.ObjectData);
        CurrencyManager.instance.UseCurrency(container.ObjectData.Price);
        Destroy(container.gameObject);
        onItemPurchased?.Invoke();
    }
}