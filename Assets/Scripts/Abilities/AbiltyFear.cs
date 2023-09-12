using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityFear", menuName = "Abilities/Fear")]
public class AbiltyFear : AbilityBase
{
    public GameObject AbilityGraphics;
    public float AbilityRange;
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
            enemy.ReceiveTempFearEffect(3);
        }
    }
}
