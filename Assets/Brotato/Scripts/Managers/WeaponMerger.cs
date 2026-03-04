using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponMerger : MonoBehaviour
{
    public static WeaponMerger instance;

    [Header(" Elements ")]
    [SerializeField] private PlayerWeapons playerWeapons;

    [Header(" Settings ")]
    private List<Weapon> weaponsToMerge = new List<Weapon>();


    [Header(" Actions ")]
    public static Action<Weapon> onMerge;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public bool CanMerge(Weapon weapon)
    {
        if (weapon.Level >= 3)
            return false;

        weaponsToMerge.Clear();
        weaponsToMerge.Add(weapon);

        Weapon[] weapons = playerWeapons.GetWeapons();

        foreach(Weapon playerWeapon in weapons)
        {
            // We can't merge with a null weapon
            if (playerWeapon == null)
                continue;

            // We can't merge a weapon with itself
            if (playerWeapon == weapon)
                continue;

            // Not the same weapons
            if (playerWeapon.WeaponData.Name != weapon.WeaponData.Name)
                continue;

            // We can't merge same weapons with different levels
            if (playerWeapon.Level != weapon.Level)
                continue;

            weaponsToMerge.Add(playerWeapon);

            return true;
        }

        return false;
    }

    public void Merge()
    {
        if(weaponsToMerge.Count < 2)
        {
            Debug.LogError("Something went wrong here...");
            return;
        }

        DestroyImmediate(weaponsToMerge[1].gameObject);

        weaponsToMerge[0].Upgrade();

        Weapon weapon = weaponsToMerge[0];
        weaponsToMerge.Clear();

        onMerge?.Invoke(weapon);
    }
}
