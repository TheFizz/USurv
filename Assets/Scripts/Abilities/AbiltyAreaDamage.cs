using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbiltyAreaDamage", menuName = "Abilities/AreaDamage")]
public class AbiltyAreaDamage : AbilityBase
{
    public override void Use(Transform source)
    {
        var sourceFloored = source.position;
        sourceFloored.y = 0;

        Instantiate(AbilityGraphics, source.position, Quaternion.identity);

        Collider[] hitEnemies = Physics.OverlapSphere(sourceFloored, Stats.Find(x => x.Parameter == StatParam.AbilityRange).Value, TargetLayer);
        foreach (var hitEnemy in hitEnemies)
        {
            var enemy = hitEnemy.GetComponent<NewEnemyBase>();
            enemy.Damage(Stats.Find(x => x.Parameter == StatParam.AbilityPower).Value, false, "PLAYER");

            foreach (var trinket in Game.PSystems.CurrentTrinkets)
            {
                if (trinket is OnAbilityEffectTrinketSO)
                {
                    var OHTrinket = (OnAbilityEffectTrinketSO)trinket;
                    OHTrinket.OnHitAction(enemy);
                }
            }
        }
    }
}
