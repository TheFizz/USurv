using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "WeaponRewardPool")]
public class WeaponRewardPoolSO : ScriptableObject
{
    public List<WeaponBaseSO> RewardPool = new List<WeaponBaseSO>();
}
