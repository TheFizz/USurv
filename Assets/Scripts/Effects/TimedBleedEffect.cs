using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBleedEffect : TimedEffect
{
    IDamageable _target;
    public TimedBleedEffect(EffectSO EffectData, IDamageable target) : base(EffectData)
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
        BleedEffectSO bleedEffect = (BleedEffectSO)EffectData;
        var damage = bleedEffect.DamagePerStack * EffectStacks;
        _target.Damage(damage, false, "PLAYER");
    }
}
