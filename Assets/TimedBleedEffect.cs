using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBleedEffect : TimedEffect
{
    NewEnemyBase _enemy;
    public TimedBleedEffect(EffectSO EffectData, NewEnemyBase enemy) : base(EffectData, enemy)
    {
        _enemy = enemy;
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
        _enemy.Damage(damage, false);
    }
}
