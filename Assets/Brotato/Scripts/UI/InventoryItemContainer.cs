using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro; 

public class InventoryItemContainer : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Image container;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI quantityText; 

    public int Index { get; private set; }
    public Weapon Weapon { get; private set; }
    public ObjectDataSO ObjectData { get; private set; }

    public void Configure(Color containerColor, Sprite itemIcon)
    {
        container.color = containerColor;
        icon.sprite = itemIcon;
    }

    public void Configure(Weapon weapon, int index, Action clickedCallback)
    {
        Weapon = weapon;
        Index = index;

        Color color = ColorHolder.GetColor(weapon.Level);
        Sprite icon = weapon.WeaponData.Sprite;

        Configure(color, icon);

        // Ẩn text số lượng đối với vũ khí
        if (quantityText != null) quantityText.gameObject.SetActive(false);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => clickedCallback?.Invoke());
    }

    
    public void Configure(ObjectDataSO objectData, int quantity, Action clickedCallback)
    {
        ObjectData = objectData;
        Color color = ColorHolder.GetColor(objectData.Rarity);
        Sprite icon = objectData.Icon;

        Configure(color, icon);

        if (quantityText != null)
        {
            if (quantity > 1)
            {
                quantityText.text = "x" + quantity;
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.gameObject.SetActive(false); 
            }
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => clickedCallback?.Invoke());
    }
}