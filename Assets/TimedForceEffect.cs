using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedForceEffect : TimedEffect
{
    NewEnemyBase _enemy;
    ForceData _forceData;
    float _massDelta;
    public TimedForceEffect(EffectSO EffectData, NewEnemyBase enemy, ForceData forceData) : base(EffectData, enemy)
    {
        _enemy = enemy;
        _forceData = forceData;
    }

    protected override void ApplyEffect()
    {

        var mass = _enemy.GetMass();
        var newMass = mass * (1f + (_forceData.MassIncreasePerc / 100));
        _massDelta = newMass - mass;
        _enemy.SetMass(newMass);
        ForceEffectSO forceEffect = (ForceEffectSO)EffectData;
        var sourceFloored = new Vector3(_forceData.ForceSource.x, 0, _forceData.ForceSource.z);
        var enemyPos = _enemy.transform.position;
        var enemyPosFloored = new Vector3(enemyPos.x, 0, enemyPos.z);
        Vector3 dirSrcToEnemy = (enemyPosFloored - sourceFloored).normalized;
        _enemy.ForceVector = dirSrcToEnemy * _forceData.ForceStrength;

    }

    public override void End()
    {
        _enemy.ForceVector = Vector3.zero;
        _enemy.SetMass(_enemy.GetMass() - _massDelta);
    }
}
