using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerStatsManager))]
public class PlayerObjects : MonoBehaviour
{
    [field: SerializeField] public List<ObjectDataSO> Objects { get; private set; }
    private PlayerStatsManager playerStatsManager;

    private void Awake()
    {
        playerStatsManager = GetComponent<PlayerStatsManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (ObjectDataSO objectData in Objects)
            playerStatsManager.AddObject(objectData.BaseStats);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddObject(ObjectDataSO objectData)
    {
        Debug.Log("Adding object from playerObject : " + objectData.BaseStats.Count);

        Objects.Add(objectData);
        playerStatsManager.AddObject(objectData.BaseStats);
    }
    
    public void RecycleObject(ObjectDataSO objectData)
    {
        // Remove object from objects list
        Objects.Remove(objectData);

        // Get the money back from the currencyManager
        CurrencyManager.instance.AddCurrency(objectData.RecyclePrice);

        // Remove object stats from the player stats manager
        playerStatsManager.RemoveObjectStats(objectData.BaseStats);
    }
}
