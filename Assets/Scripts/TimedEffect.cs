using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedEffect
{
    protected float Duration;
    protected int EffectStacks;
    protected float ProcTime;
    private float _timeToProc;
    private int _procTotal;
    private int _procCount;
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
        var p = Duration / ProcTime;
        if (Duration <= 0)
        {
            End();
            IsFinished = true;
        }
        else if (_timeToProc <= 0)
        {
            Proc();
            _timeToProc = ProcTime + _timeToProc;
            _procCount++;
        }
    }

    public void Activate()
    {
        ProcTime = EffectData.ProcTime;

        if (EffectData.IsEffectStacked || Duration <= 0)
        {
            if (EffectStacks < EffectData.MaxStacks)
            {
                ApplyEffect();
                EffectStacks++;
            }
        }

        if (Duration <= 0)
            Duration = EffectData.Duration;
        else
        {
            if (EffectData.IsDurationStacked)
                if (EffectStacks < EffectData.MaxStacks)
                {
                    Duration += EffectData.Duration;
                    EffectStacks++;
                }
            if (EffectData.IsDurationRenewed)
                Duration = EffectData.Duration;
        }
    }
    protected abstract void ApplyEffect();
    public abstract void End();
    protected virtual void Proc() { }
}
