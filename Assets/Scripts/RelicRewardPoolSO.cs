using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "RelicRewardPool")]
public class RelicRewardPoolSO : ScriptableObject
{
    public List<object> RewardPool = new List<object>();
}
