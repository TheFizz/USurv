using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RewardGenerator : MonoBehaviour
{
    public List<RewardPillar> Pillars = new List<RewardPillar>();

    public TrinketRewardPoolSO TrinketRewards;
    public WeaponRewardPoolSO WeaponRewards;
    public RelicRewardPoolSO RelicRewards;
    public void InstantiateRewards()
    {
        var wReward = WeaponRewards.RewardPool[Random.Range(0, WeaponRewards.RewardPool.Count)];

        var tReward = TrinketRewards.RewardPool[Random.Range(0, TrinketRewards.RewardPool.Count)];


        Instantiate(wReward.PickupObject).transform.position = Pillars[0].SpawnPoint.position;
        Pillars[0].Interactive.AddComponent<PillarInteraction>().Title = $"Pick {wReward.WeaponName}";


        Instantiate(tReward.PickupObject).transform.position = Pillars[1].SpawnPoint.position;
        Pillars[1].Interactive.AddComponent<PillarInteraction>().Title = $"Pick {tReward.Name}";

        //var rReward = RelicRewards.RewardPool[Random.Range(0, RelicRewards.RewardPool.Count)];
        /*
        // 0=W 1=T 2=R
        var auxPool = Random.Range(0, 3);
        string[] auxLines = null;
        switch (auxPool)
        {
            case 0:
                do
                {
                    auxReward = auxLines[Random.Range(0, rLines.Length)];
                }
                while (auxReward == wReward ||
             auxReward == tReward ||
             auxReward == rReward);
                break;
            case 1:
                auxLines = TrinketRewards.text.Split(Environment.NewLine);
                break;
            case 2:
                auxLines = RelicRewards.text.Split(Environment.NewLine);
                break;
        }
        string auxReward;
        

        Debug.Log($"{wReward}\n{tReward}\n{rReward}\n{auxReward}");
        */
    }
}
