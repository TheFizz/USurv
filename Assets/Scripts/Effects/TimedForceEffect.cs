using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedForceEffect : TimedEffect
{
    IForceable _target;
    ForceData _forceData;
    float _massDelta;
    public TimedForceEffect(EffectSO EffectData, IForceable target, ForceData forceData) : base(EffectData)
    {
        _target = target;
        _forceData = forceData;
    }

    protected override void ApplyEffect()
    {
        var mass = _target.GetMass();
        var newMass = mass * (1f + (_forceData.MassIncreasePerc / 100));
        _massDelta = newMass - mass;
        _target.SetMass(newMass);
        ForceEffectSO forceEffect = (ForceEffectSO)EffectData;
        var sourceFloored = new Vector3(_forceData.ForceSource.x, 0, _forceData.ForceSource.z);
        var enemyPos = _target.GetTransform().position;
        var enemyPosFloored = new Vector3(enemyPos.x, 0, enemyPos.z);
        Vector3 dirSrcToEnemy = (enemyPosFloored - sourceFloored).normalized;
        _target.ForceVector = dirSrcToEnemy * _forceData.ForceStrength;
    }

    public override void End()
    {
        _target.ForceVector = Vector3.zero;
        _target.SetMass(_target.GetMass() - _massDelta);
    }
}
