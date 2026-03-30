using System;
using UnityEngine;

public class RunLoader : MonoBehaviour
{
    [Header(" References ")]
    [SerializeField] private PlayerStatsManager playerStatsManager;
    [SerializeField] private PlayerWeapons playerWeapons;
    [SerializeField] private PlayerObjects playerObjects;
    [SerializeField] private PlayerLevel playerLevel;

    // Xóa Start() và IEnumerator TryLoad()

    // THÊM: Nạp dữ liệu một cách chủ động
    public void LoadAndContinue()
    {
        if (SaveManager.instance == null || !SaveManager.instance.HasSave())
            return;

        RunSaveData data = SaveManager.instance.LoadRun();
        if (data == null) return;

        Apply(data);
    }

    private void Apply(RunSaveData data)
    {
        // (Giữ nguyên toàn bộ ruột của hàm Apply cũ)
        if (WaveManager.instance != null)
            WaveManager.instance.LoadFromSave(data.currentWaveIndex, data.isEndlessMode, data.difficultyMultiplier);

        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.UseCurrency(CurrencyManager.instance.Currency);
            CurrencyManager.instance.AddCurrency(data.currency);
        }

        if (playerLevel != null)
            playerLevel.LoadFromSave(data.playerLevel, data.currentXp);

        if (playerStatsManager != null)
        {
            foreach (StatEntry entry in data.statAddends)
            {
                if (Enum.TryParse<Stat>(entry.statName, out Stat stat))
                    playerStatsManager.AddPlayerStat(stat, entry.value);
            }
        }

        if (playerWeapons != null)
        {
            foreach (WeaponSaveEntry entry in data.weapons)
            {
                WeaponDataSO weaponData = Resources.Load<WeaponDataSO>("Data/Weapons/" + entry.weaponDataName);
                if (weaponData == null) continue;
                playerWeapons.TryAddWeaponAtSlot(weaponData, entry.level, entry.slotIndex);
            }
        }

        if (playerObjects != null)
        {
            foreach (string objName in data.objectNames)
            {
                ObjectDataSO objectData = Resources.Load<ObjectDataSO>("Data/Objects/" + objName);
                if (objectData == null) continue;
                playerObjects.AddObject(objectData);
            }
        }

        Debug.Log($"[RunLoader] Save applied: wave {data.currentWaveIndex + 1}");
    }
}