using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsDisplay : MonoBehaviour, IPlayerStatsDependency
{
    [Header(" Elements ")]
    [SerializeField] private Transform playerStatContainersParent;

    public void UpdateStats(PlayerStatsManager playerStatsManager)
    {
        int index = 0;

        foreach(Stat stat in Enum.GetValues(typeof(Stat)))
        {
            StatContainer statContainer = playerStatContainersParent.GetChild(index).GetComponent<StatContainer>();
            statContainer.gameObject.SetActive(true);

            Sprite statIcon = ResourcesManager.GetStatIcon(stat);

            float statValue = playerStatsManager.GetStatValue(stat);

            statContainer.Configure(statIcon, Enums.FormatStatName(stat), statValue, true);

            index++;
        }

        for (int i = index; i < playerStatContainersParent.childCount; i++)
            playerStatContainersParent.GetChild(i).gameObject.SetActive(false);        
    }
}
