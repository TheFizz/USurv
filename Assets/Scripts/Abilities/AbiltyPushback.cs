using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbiltyPushback", menuName = "Abilities/Pushback")]
public class AbiltyPushback : AbilityBase
{
    public EffectSO PushbackEffect;
    public override void Use(Transform source)
    {
        var sourceFloored = source.position;
        sourceFloored.y = 0;

        Instantiate(AbilityGraphics, source.position, Quaternion.identity);

        Collider[] hitEnemies = Physics.OverlapSphere(sourceFloored, GetStat(StatParam.AbilityRange).Value, TargetLayer);
        var forceStrength = GetStat(StatParam.AbilityPower).Value;
        foreach (var hitEnemy in hitEnemies)
        {
            var enemy = hitEnemy.GetComponent<NewEnemyBase>();
            enemy.AddEffect(PushbackEffect.InitializeEffect(enemy, new ForceData(source.position, forceStrength, 0)));
        }
    }
}
