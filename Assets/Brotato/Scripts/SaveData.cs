using System;
using System.Collections.Generic;

[Serializable]
public class RunSaveData
{   
    public string savedGameState = "SHOP";
    public int currentWaveIndex;
    public bool isEndlessMode;
    public float difficultyMultiplier;
    public int currency;
    public int playerLevel;
    public int currentXp;
    public List<StatEntry> statAddends   = new List<StatEntry>();
    public List<WeaponSaveEntry> weapons = new List<WeaponSaveEntry>();
    public List<string> objectNames      = new List<string>();
    public string savedAt;
}

[Serializable]
public class StatEntry
{
    public string statName;
    public float value;
}

[Serializable]
public class WeaponSaveEntry
{
    public string weaponDataName;
    public int level;
    public int slotIndex;
}