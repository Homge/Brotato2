using System;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private const string RUN_SAVE_KEY = "CurrentRunSave";
    private const string HAS_SAVE_KEY = "HasActiveSave";

    private void Awake()
    {
      instance = this;
    }

    // ── PUBLIC ──────────────────────────────────────────────────

    public void SaveRun()
    {
        RunSaveData data = new RunSaveData();
        data.savedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        CollectWaveData(data);
        CollectCurrencyData(data);
        CollectPlayerData(data);

        if (GameManager.instance != null)
            data.savedGameState = GameManager.instance.CurrentState.ToString();

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(RUN_SAVE_KEY, json);
        PlayerPrefs.SetInt(HAS_SAVE_KEY, 1);
        PlayerPrefs.Save();

        Debug.Log($"[SaveManager] Saved at wave {data.currentWaveIndex + 1}");
    }

    public RunSaveData LoadRun()
    {
        if (!HasSave()) return null;

        string json = PlayerPrefs.GetString(RUN_SAVE_KEY);
        return JsonUtility.FromJson<RunSaveData>(json);
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(RUN_SAVE_KEY);
        PlayerPrefs.SetInt(HAS_SAVE_KEY, 0);
        PlayerPrefs.Save();
        Debug.Log("[SaveManager] Save deleted.");
    }

    public bool HasSave() => PlayerPrefs.GetInt(HAS_SAVE_KEY, 0) == 1;

    // ── COLLECT ─────────────────────────────────────────────────

    private void CollectWaveData(RunSaveData data)
    {
        if (WaveManager.instance == null) return;
        data.currentWaveIndex     = WaveManager.instance.CurrentWaveIndex;
        data.isEndlessMode        = WaveManager.instance.IsEndlessMode;
        data.difficultyMultiplier = WaveManager.DifficultyMultiplier;
    }

    private void CollectCurrencyData(RunSaveData data)
    {
        if (CurrencyManager.instance == null) return;
        data.currency = CurrencyManager.instance.Currency;
    }

    private void CollectPlayerData(RunSaveData data)
    {
        // Level & XP
        PlayerLevel pl = FindFirstObjectByType<PlayerLevel>();
        if (pl != null)
        {
            data.playerLevel = pl.CurrentLevel;
            data.currentXp   = pl.CurrentXp;
        }

        // Stat addends (upgrades từ wave transition)
        PlayerStatsManager psm = FindFirstObjectByType<PlayerStatsManager>();
        if (psm != null)
        {
            foreach (var kvp in psm.GetAddends())
                data.statAddends.Add(new StatEntry { statName = kvp.Key.ToString(), value = kvp.Value });
        }

        // Weapons
        PlayerWeapons pw = FindFirstObjectByType<PlayerWeapons>();
        if (pw != null)
        {
            Weapon[] weapons = pw.GetWeapons();
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] == null) continue;
                data.weapons.Add(new WeaponSaveEntry
                {
                    weaponDataName = weapons[i].WeaponData.name,
                    level          = weapons[i].Level,
                    slotIndex      = i
                });
            }
        }

        // Objects / items
        PlayerObjects po = FindFirstObjectByType<PlayerObjects>();
        if (po != null)
            foreach (var obj in po.Objects)
                data.objectNames.Add(obj.name);
    }
}