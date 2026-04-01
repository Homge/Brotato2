using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IGameStateListener
{
    [Header(" Player Components ")]
    [SerializeField] private PlayerWeapons playerWeapons;
    [SerializeField] private PlayerObjects playerObjects;

    [Header(" Elements ")]
    [SerializeField] private Transform inventoryItemsParent;
    [SerializeField] private Transform pauseInventoryItemsParent;
    [SerializeField] private InventoryItemContainer inventoryItemContainer;
    [SerializeField] private ShopManagerUI shopManagerUI;
    [SerializeField] private InventoryItemInfo itemInfo;

    private void Awake()
    {
        ShopManager.onItemPurchased += ItemPurchasedCallback;
        WeaponMerger.onMerge += WeaponMergedCallback;

        GameManager.onGamePaused += Configure;
    }

    private void OnDestroy()
    {
        ShopManager.onItemPurchased -= ItemPurchasedCallback;
        WeaponMerger.onMerge -= WeaponMergedCallback;

        GameManager.onGamePaused -= Configure;
    }





    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState == GameState.SHOP)
            Configure();
    }

    private void Configure()
    {
        inventoryItemsParent.Clear();
        pauseInventoryItemsParent.Clear();

        // 1. VŨ KHÍ (Giữ nguyên không gộp)
        Weapon[] weapons = playerWeapons.GetWeapons();
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
                continue;
            InventoryItemContainer container = Instantiate(inventoryItemContainer, inventoryItemsParent);
            container.Configure(weapons[i], i, () => ShowItemInfo(container));

            InventoryItemContainer pauseContainer = Instantiate(inventoryItemContainer, pauseInventoryItemsParent);
            pauseContainer.Configure(weapons[i], i, null);
        }

        // 2. VẬT PHẨM (Gộp lại bằng Dictionary)
        Dictionary<ObjectDataSO, int> itemCounts = new Dictionary<ObjectDataSO, int>();
        
        // Đếm số lượng từng vật phẩm
        foreach (ObjectDataSO obj in playerObjects.Objects)
        {
            if (itemCounts.ContainsKey(obj))
                itemCounts[obj]++;
            else
                itemCounts.Add(obj, 1);
        }

        // Khởi tạo UI dựa trên danh sách đã gộp
        foreach (KeyValuePair<ObjectDataSO, int> kvp in itemCounts)
        {
            ObjectDataSO itemData = kvp.Key;
            int quantity = kvp.Value;

            InventoryItemContainer container = Instantiate(inventoryItemContainer, inventoryItemsParent);
            container.Configure(itemData, quantity, () => ShowItemInfo(container));

            InventoryItemContainer pauseContainer = Instantiate(inventoryItemContainer, pauseInventoryItemsParent);
            pauseContainer.Configure(itemData, quantity, null);
        }        
    }

    private void ShowItemInfo(InventoryItemContainer container)
    {
        if (container.Weapon != null)
            ShowWeaponInfo(container.Weapon, container.Index);
        else
            ShowObjectInfo(container.ObjectData);
    }

    private void ShowWeaponInfo(Weapon weapon, int index)
    {
        itemInfo.Configure(weapon);

        itemInfo.RecycleButton.onClick.RemoveAllListeners();
        itemInfo.RecycleButton.onClick.AddListener(() => RecycleWeapon(index));

        shopManagerUI.ShowItemInfo();
    }

    private void RecycleWeapon(int index)
    {
        playerWeapons.RecycleWeapon(index);

        Configure();

        shopManagerUI.HideItemInfo();

        Debug.Log("Recycling weapon at index " + index);
    }

    private void ShowObjectInfo(ObjectDataSO objectData)
    {
        itemInfo.Configure(objectData);

        itemInfo.RecycleButton.onClick.RemoveAllListeners();
        itemInfo.RecycleButton.onClick.AddListener(() => RecycleObject(objectData));

        shopManagerUI.ShowItemInfo();
    }

    private void RecycleObject(ObjectDataSO objectToRecycle)
    {
        // Remove the Object from PlayerObjects
        playerObjects.RecycleObject(objectToRecycle);

        // Destroy the inventory item container
        Configure();

        // Close the item info
        shopManagerUI.HideItemInfo();
    }

    private void ItemPurchasedCallback() => Configure();

    private void WeaponMergedCallback(Weapon mergedWeapon)
    {
        Configure();
        itemInfo.Configure(mergedWeapon);
    }
}
