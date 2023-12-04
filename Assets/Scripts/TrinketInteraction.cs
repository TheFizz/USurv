using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrinketInteraction : Interaction
{
    public TrinketRewardPoolSO pool;
    private TrinketSO _trinket;
    private void Awake()
    {
        Options = new List<Tuple<KeyCode, InteractionType>>
        {
            new Tuple<KeyCode, InteractionType>(KeyCode.F, InteractionType.Take),
        };
    }
    private void Start()
    {
        if (_trinket != null)
            return;
        _trinket = pool.RewardPool[Random.Range(0, pool.RewardPool.Count)];
        InteractionTitle = _trinket.Name;
    }
    public TrinketSO GetTrinket()
    {
        return _trinket;
    }
    public void SetTrinket(TrinketSO trinket)
    {
        _trinket = trinket;
        InteractionTitle = _trinket.Name;
    }
}
