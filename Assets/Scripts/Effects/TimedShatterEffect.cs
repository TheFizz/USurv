using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedShatterEffect : TimedEffect
{
    IDamageable _target;
    private bool isApplied;
    private float _ampDelta;

    EnemyVFXAnchors _anchors;
    GameObject _effect;

    public TimedShatterEffect(EffectSO EffectData, IDamageable target) : base(EffectData)
    {
        _target = target;
        _anchors = ((MonoBehaviour)_target).gameObject.GetComponent<EnemyVFXAnchors>();
    }

    protected override void ApplyEffect()
    {
        ShatterEffectSO shatterEffect = (ShatterEffectSO)EffectData;
        if (!isApplied)
            _ampDelta = shatterEffect.RecvDamageAmpPerc / 100;
        _target.InDmgFactor += _ampDelta;
        isApplied = true;

        if (_effect == null)
            _effect = Object.Instantiate(EffectData.EffectPrefab, _anchors.chestAnchor);
    }

    public override void End()
    {
        _target.InDmgFactor -= (_ampDelta * EffectStacks);
        EffectStacks = 0;
        isApplied = false;

        Object.Destroy(_effect);
    }
}
