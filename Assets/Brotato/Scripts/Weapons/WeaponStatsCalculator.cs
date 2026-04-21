using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeaponStatsCalculator 
{
    public static Dictionary<Stat, float> GetStats(WeaponDataSO weaponData, int level)
    {
        float multiplier = 1 + (float)level / 3;
        Dictionary<Stat, float> calculatedStats = new Dictionary<Stat, float>();

        foreach(KeyValuePair<Stat, float> kvp in weaponData.BaseStats)
        {
            if (weaponData.Prefab.GetType() != typeof(RangeWeapon) && kvp.Key == Stat.Range)
                calculatedStats.Add(kvp.Key, kvp.Value);
            else if (kvp.Key == Stat.CriticalPercent || kvp.Key == Stat.CriticalChance)
                calculatedStats.Add(kvp.Key, kvp.Value);
            else
                calculatedStats.Add(kvp.Key, kvp.Value * multiplier);
        }

        return calculatedStats;
    }

    public static int GetPurchasePrice(WeaponDataSO weaponData, int level)
    {
        float levelMultiplier = 1 + (float)level / 3;
        
        int waveIndex = WaveManager.instance != null ? WaveManager.instance.CurrentWaveIndex : 0;
        float waveProgressMultiplier = 1 + (waveIndex * 0.1f); 

        return Mathf.RoundToInt(weaponData.PurchasePrice * levelMultiplier * waveProgressMultiplier);
    }

    public static int GetRecyclePrice(WeaponDataSO weaponData, int level)
    {
        float multiplier = 1 + (float)level / 3;
        return (int)(weaponData.RecyclePrice * multiplier);
    }
}