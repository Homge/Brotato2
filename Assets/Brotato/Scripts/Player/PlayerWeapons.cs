using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private WeaponPosition[] weaponPositions;

    [Header(" Synergies ")]
    [SerializeField] private SynergyDefinition[] availableSynergies;
    [SerializeField] private PlayerStatsManager playerStatsManager;

    void Start() { }
    void Update() { }

    public bool TryAddWeapon(WeaponDataSO weapon, int level)
    {
        for (int i = 0; i < weaponPositions.Length; i++)
        {
            if (weaponPositions[i].Weapon != null)
                continue;

            weaponPositions[i].AssignWeapon(weapon.Prefab, level);
            EvaluateSynergies();
            return true;
        }

        return false;
    }

    /// Thêm vào đúng slot chỉ định — dùng khi load save
    public bool TryAddWeaponAtSlot(WeaponDataSO weapon, int level, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= weaponPositions.Length) return false;
        if (weaponPositions[slotIndex].Weapon != null) return false;

        weaponPositions[slotIndex].AssignWeapon(weapon.Prefab, level);
        EvaluateSynergies();
        return true;
    }

    public void RecycleWeapon(int weaponIndex)
    {
        for (int i = 0; i < weaponPositions.Length; i++)
        {
            if (i != weaponIndex)
                continue;

            int recyclePrice = weaponPositions[i].Weapon.GetRecyclePrice();
            CurrencyManager.instance.AddCurrency(recyclePrice);

            weaponPositions[i].RemoveWeapon();
            EvaluateSynergies();

            return;
        }
    }

    public Weapon[] GetWeapons()
    {
        List<Weapon> weapons = new List<Weapon>();

        foreach (WeaponPosition weaponPosition in weaponPositions)
        {
            if (weaponPosition.Weapon == null)
                weapons.Add(null);
            else
                weapons.Add(weaponPosition.Weapon);
        }

        return weapons.ToArray();
    }

    public void EvaluateSynergies()
    {
        if (playerStatsManager == null) return;

        Dictionary<WeaponTag, int> tagCounts = new Dictionary<WeaponTag, int>();

        foreach (WeaponPosition wp in weaponPositions)
        {
            if (wp.Weapon != null && wp.Weapon.WeaponData != null && wp.Weapon.WeaponData.Tags != null)
            {
                foreach (WeaponTag tag in wp.Weapon.WeaponData.Tags)
                {
                    if (!tagCounts.ContainsKey(tag))
                        tagCounts[tag] = 0;

                    tagCounts[tag]++;
                }
            }
        }

        Dictionary<Stat, float> totalSynergyBonuses = new Dictionary<Stat, float>();

        if (availableSynergies != null)
        {
            foreach (SynergyDefinition syn in availableSynergies)
            {
                if (tagCounts.ContainsKey(syn.requiredTag) && tagCounts[syn.requiredTag] >= syn.requiredCount)
                {
                    if (syn.boosts != null)
                    {
                        foreach (StatBoost boost in syn.boosts)
                        {
                            if (!totalSynergyBonuses.ContainsKey(boost.stat))
                                totalSynergyBonuses[boost.stat] = 0;

                            totalSynergyBonuses[boost.stat] += boost.value;
                        }
                    }
                }
            }
        }

        playerStatsManager.UpdateSynergyStats(totalSynergyBonuses);
    }

    public Dictionary<WeaponTag, int> GetTagCounts()
    {
        Dictionary<WeaponTag, int> tagCounts = new Dictionary<WeaponTag, int>();

        foreach (WeaponPosition wp in weaponPositions)
        {
            if (wp.Weapon != null && wp.Weapon.WeaponData != null && wp.Weapon.WeaponData.Tags != null)
            {
                foreach (WeaponTag tag in wp.Weapon.WeaponData.Tags)
                {
                    if (!tagCounts.ContainsKey(tag)) tagCounts[tag] = 0;
                    tagCounts[tag]++;
                }
            }
        }

        return tagCounts;
    }

    public SynergyDefinition[] GetAvailableSynergies() => availableSynergies;
}

[System.Serializable]
public struct StatBoost
{
    public Stat stat;
    public float value;
}

[System.Serializable]
public struct SynergyDefinition
{
    public string synergyName;
    public WeaponTag requiredTag;
    public int requiredCount;
    public StatBoost[] boosts;
}