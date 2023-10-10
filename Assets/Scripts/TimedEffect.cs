using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedEffect
{
    protected float Duration;
    protected int EffectStacks;
    protected float ProcTime;
    private float _timeToProc;
    public EffectSO EffectData { get; }
    public bool IsFinished;

    public TimedEffect(EffectSO effectData, NewEnemyBase enemy)
    {
        EffectData = effectData;
    }

    public void Tick(float delta)
    {
        Duration -= delta;
        _timeToProc -= delta;
        if (_timeToProc <= 0)
        {
            Proc();
            _timeToProc = ProcTime;
        }
        if (Duration <= 0)
        {
            End();
            IsFinished = true;
        }
    }

    public void Activate()
    {
        ProcTime = EffectData.ProcTime;
        _timeToProc = ProcTime;
        if (EffectData.IsEffectStacked || Duration <= 0)
        {
            ApplyEffect();
            EffectStacks++;
        }

        if (EffectData.IsDurationStacked || Duration <= 0)
        {
            Duration += EffectData.Duration;
        }
    }
    protected abstract void ApplyEffect();
    public abstract void End();
    protected virtual void Proc() { }
}
