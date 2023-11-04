
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trinkets/OnHitAttack")]
class OnHitAttackTrinketSO : TrinketSO
{
    public enum AuxAttacker
    {
        ACTIVE = 0,
        PASSIVE = 1,
        ABILITY = 2,
    }

    public float ChancePerc;
    public AuxAttacker Aux;


    public bool Roll()
    {
        int roll = Random.Range(0, 100);
        return roll < ChancePerc;
    }

    public void OnHitAttack()
    {
        if (!Roll())
            return;
        Game.PSystems.PlayerWeapons[(int)Aux].AttackAfterDelay(0.1f);
    }
}
