using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PlayerStatsManager : MonoBehaviour
{
    [Header(" Data ")]
    [SerializeField] private CharacterDataSO playerData;

    [Header(" Settings ")]
    private Dictionary<Stat, float> playerStats = new Dictionary<Stat, float>();
    private Dictionary<Stat, float> addends = new Dictionary<Stat, float>();
    private Dictionary<Stat, float> objectAddends = new Dictionary<Stat, float>();
    private Dictionary<Stat, float> synergyAddends = new Dictionary<Stat, float>();

    private void Awake()
    {
        CharacterSelectionManager.onCharacterSelected += CharacterSelectedCallback;

        playerStats = playerData.BaseStats;

        foreach (KeyValuePair<Stat, float> kvp in playerStats)
        {
            addends.Add(kvp.Key, 0);
            objectAddends.Add(kvp.Key, 0);
            synergyAddends.Add(kvp.Key, 0);
        }
    }

    private void OnDestroy()
    {
        CharacterSelectionManager.onCharacterSelected -= CharacterSelectedCallback;
    }


    void Start() => UpdatePlayerStats();

    public void AddPlayerStat(Stat stat, float value)
    {
        if (addends.ContainsKey(stat))            
            addends[stat] += value;
        else
            Debug.LogError($"The key {stat} has not been found, this is not normal !!! Review your code !!!!");

        UpdatePlayerStats();
    }

    public void AddObject(Dictionary<Stat, float> objectStats)
    {
        foreach (KeyValuePair<Stat, float> kvp in objectStats)
            objectAddends[kvp.Key] += kvp.Value;        

        UpdatePlayerStats();
    }

    public void RemoveObjectStats(Dictionary<Stat, float> objectStats)
    {
        foreach (KeyValuePair<Stat, float> kvp in objectStats)
            objectAddends[kvp.Key] -= kvp.Value;        

        UpdatePlayerStats();
    }

    public float GetStatValue(Stat stat) => playerStats[stat] + addends[stat] + objectAddends[stat] + synergyAddends[stat];
        

    private void UpdatePlayerStats()
    {
        IEnumerable<IPlayerStatsDependency> playerStatsDependencies =
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IPlayerStatsDependency>();

        foreach (IPlayerStatsDependency dependency in playerStatsDependencies)
            dependency.UpdateStats(this);
    }


    private void CharacterSelectedCallback(CharacterDataSO characterData)
    {
        playerData = characterData;
        playerStats = playerData.BaseStats;

        UpdatePlayerStats();
    }
    public void UpdateSynergyStats(Dictionary<Stat, float> newSynergyStats)
{
    List<Stat> keys = new List<Stat>(synergyAddends.Keys);
    foreach (Stat key in keys)
    {
        synergyAddends[key] = 0;
    }

    foreach (KeyValuePair<Stat, float> kvp in newSynergyStats)
    {
        if (synergyAddends.ContainsKey(kvp.Key))
            synergyAddends[kvp.Key] += kvp.Value;
    }

    UpdatePlayerStats();
}
}
