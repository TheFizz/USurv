using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectSO : ScriptableObject
{
    public int MaxStacks = 1;
    public float Duration;
    public bool IsDurationStacked;
    public bool IsDurationRenewed;
    public bool IsEffectStacked;
    public float ProcTime = 0.5f;

    public abstract TimedEffect InitializeEffect(NewEnemyBase enemy, object auxData = null);

}
