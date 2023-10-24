
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trinkets/OnHitStackTimed")]
public class OnHitStackTimedTrinketSO : TrinketSO
{
    public float Duration;
    public int MaxStacks;
    public StatModifier StackBonus;

    public OnHitStackTimedTrinket Init()
    {
        return new OnHitStackTimedTrinket(this);
    }
}
