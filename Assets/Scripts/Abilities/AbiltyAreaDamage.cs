using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbiltyAreaDamage", menuName = "Abilities/AreaDamage")]
public class AbiltyAreaDamage : AbilityBase
{
    public GameObject AbilityGraphics;
    public float AbilityRange;
    public float AbilityDamage;
    public LayerMask TargetLayer;

    public override void Use(Transform source)
    {
        var sourceFloored = source.position;
        sourceFloored.y = 0;

        Instantiate(AbilityGraphics, source.position, Quaternion.identity);

        Collider[] hitEnemies = Physics.OverlapSphere(sourceFloored, AbilityRange, TargetLayer);
        foreach (var hitEnemy in hitEnemies)
        {
            var enemy = hitEnemy.GetComponent<EnemyBase>();
            enemy.Damage(AbilityDamage);
        }
    }
}
