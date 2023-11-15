using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPrayerEffect : TimedEffect
{
    IDamageable _target;
    private bool isApplied;
    private float _ampDelta;

    EnemyVFXAnchors _anchors;
    GameObject _effect;

    public TimedPrayerEffect(EffectSO EffectData, IDamageable target) : base(EffectData)
    {
        _target = target;
        _anchors = ((MonoBehaviour)_target).gameObject.GetComponent<EnemyVFXAnchors>();
    }

    protected override void ApplyEffect()
    {

        if (_effect == null)
            _effect = Object.Instantiate(EffectData.EffectPrefab, _anchors.headAnchor);
    }

    public override void End()
    {
        var damage = ((PrayerEffectSO)EffectData).DamagePerStack * EffectStacks;
        _target.Damage(damage, false, "PLAYER");

        Object.Destroy(_effect);
    }
}
