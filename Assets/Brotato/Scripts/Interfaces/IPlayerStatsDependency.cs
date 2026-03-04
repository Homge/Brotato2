using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerStatsDependency 
{
    void UpdateStats(PlayerStatsManager playerStatsManager);
}
