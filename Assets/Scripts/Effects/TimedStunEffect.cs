using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedStunEffect : TimedEffect
{

    IStunnable _target;
    EnemyVFXAnchors _anchors;
    GameObject _effect;
    public TimedStunEffect(EffectSO EffectData, IStunnable target) : base(EffectData)
    {
        _target = target;
        _anchors = ((MonoBehaviour)_target).gameObject.GetComponent<EnemyVFXAnchors>();
    }

    protected override void ApplyEffect()
    {
        _target.SetStunned(true);
        if (_effect == null)
            _effect = Object.Instantiate(EffectData.EffectPrefab, _anchors.headAnchor);
    }

    public override void End()
    {
        _target.SetStunned(false);
        Object.Destroy(_effect);
    }
}
