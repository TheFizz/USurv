using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBurnEffect : TimedEffect
{
    IDamageable _target;
    public TimedBurnEffect(EffectSO EffectData, IDamageable target) : base(EffectData)
    {
        _target = target;
    }

    protected override void ApplyEffect()
    {
    }

    public override void End()
    {
    }
    protected override void Proc()
    {
        BurnEffectSO burnEffect = (BurnEffectSO)EffectData;
        _target.Damage(burnEffect.DamagePerProc, false, "PLAYER");
    }
}
