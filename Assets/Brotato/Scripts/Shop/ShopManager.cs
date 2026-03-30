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
    [SerializeField] private int rerollPrice;
    [SerializeField] private TextMeshProUGUI rerollPriceText;

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
            Configure();
            UpdateRerollVisuals();
        }
    }

    // ── CONFIGURE ────────────────────────────────────────────────────────────

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

        // Tỉ lệ vũ khí : vật phẩm = 2 : 2
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

    // ── RARITY SYSTEM ────────────────────────────────────────────────────────

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

    /// <summary>
    /// Bảng trọng số theo giai đoạn wave + luck.
    /// Index: 0 = Common, 1 = Uncommon, 2 = Rare, 3 = Epic
    /// Luck dịch chuyển trọng số từ Common sang các tier cao hơn.
    /// Mỗi 10 luck = -3% Common, +1% mỗi tier còn lại (tối đa shift 30%).
    /// </summary>
    private int RollWeightedLevel(int waveIndex, bool isEndless, float luck)
    {
        float[] weights;

        if (isEndless)
            weights = new float[] { 15f, 30f, 35f, 20f };
        else if (waveIndex < 3)
            weights = new float[] { 70f, 22f, 7f, 1f };
        else if (waveIndex < 7)
            weights = new float[] { 45f, 35f, 15f, 5f };
        else
            weights = new float[] { 25f, 35f, 28f, 12f };

        // Luck shift: mỗi 10 luck dịch tối đa 3% từ Common sang các tier cao
        // Giới hạn shift tối đa 30% để không phá vỡ cân bằng
        float shift = Mathf.Clamp(luck / 10f * 3f, 0f, 30f);

        // Lấy từ Common (index 0), phân đều cho Uncommon/Rare/Epic
        float actualShift = Mathf.Min(shift, weights[0] * 0.8f); // không lấy quá 80% Common
        weights[0] -= actualShift;
        weights[1] += actualShift * 0.5f;
        weights[2] += actualShift * 0.35f;
        weights[3] += actualShift * 0.15f;

        float total = 0f;
        foreach (float w in weights) total += w;

        float roll       = Random.Range(0f, total);
        float cumulative = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative) return i;
        }

        return 0;
    }

    // ── REROLL ───────────────────────────────────────────────────────────────

    public void Reroll()
    {
        Configure();
        CurrencyManager.instance.UseCurrency(rerollPrice);
    }

    private void UpdateRerollVisuals()
    {
        rerollPriceText.text      = rerollPrice.ToString();
        rerollButton.interactable = CurrencyManager.instance.HasEnoughCurrency(rerollPrice);
    }

    private void CurrencyUpdatedCallback() => UpdateRerollVisuals();

    // ── PURCHASE ─────────────────────────────────────────────────────────────

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