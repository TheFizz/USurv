using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseSO : ScriptableObject
{
    [Header("Base Fields")]
    public float HealthMin;
    public float HealthMax;
    public float MoveSpeed;

    public float AttackSpeed;
    public float AttackRange;
    public float AttackDamage;
}