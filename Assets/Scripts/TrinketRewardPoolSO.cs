using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "TrinketRewardPool")]
public class TrinketRewardPoolSO : ScriptableObject
{
    public List<TrinketSO> RewardPool = new List<TrinketSO>();
}
