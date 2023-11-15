using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBleedEffect : TimedEffect
{
    IDamageable _target;
    EnemyVFXAnchors _anchors;
    GameObject _effect;
    public TimedBleedEffect(EffectSO EffectData, IDamageable target) : base(EffectData)
    {
        _target = target;
        _anchors = ((MonoBehaviour)_target).gameObject.GetComponent<EnemyVFXAnchors>();
    }

    protected override void ApplyEffect()
    {
        if (_effect == null)
            _effect = Object.Instantiate(EffectData.EffectPrefab, _anchors.chestAnchor);
    }

    public override void End()
    {
        Object.Destroy(_effect);
    }
    protected override void Proc()
    {
        BleedEffectSO bleedEffect = (BleedEffectSO)EffectData;
        var damage = bleedEffect.DamagePerStack * EffectStacks;
        _target.Damage(damage, false, "PLAYER");
    }
}
