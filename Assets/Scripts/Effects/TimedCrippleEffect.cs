using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedCrippleEffect : TimedEffect
{
    IDamaging _target;
    private bool isApplied;
    private float _multDelta;

    EnemyVFXAnchors _anchors;
    GameObject _effectR, _effectL;
    public TimedCrippleEffect(EffectSO EffectData, IDamaging target) : base(EffectData)
    {
        _target = target;
        _anchors = ((MonoBehaviour)_target).gameObject.GetComponent<EnemyVFXAnchors>();
    }

    protected override void ApplyEffect()
    {
        CrippleEffectSO crippleEffect = (CrippleEffectSO)EffectData;
        if (!isApplied)
            _multDelta = crippleEffect.DamageMultiplierPerc / 100;
        _target.OutDmgFactor += _multDelta;
        isApplied = true;

        if (_effectR == null)
            _effectR = Object.Instantiate(EffectData.EffectPrefab, _anchors.handAnchor[0]);
        if (_effectL == null)
            _effectL = Object.Instantiate(EffectData.EffectPrefab, _anchors.handAnchor[1]);
    }

    public override void End()
    {
        _target.OutDmgFactor -= (_multDelta * EffectStacks);
        EffectStacks = 0;
        isApplied = false;


        Object.Destroy(_effectR);
        Object.Destroy(_effectL);
    }
}
