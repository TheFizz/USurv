using Kryz.CharacterStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBaseSO : ScriptableObject
{
    [Header("Base Fields")]
    public Sprite WeaponSprite;
    public bool AimAssist = false;
    public LayerMask EnemyLayer;
    public string WeaponName;

    //public float AttacksPerSecond;
    //public float AttackDamage;
    //public float AttackArc;
    //public float AttackRange;

    public Stat AttacksPerSecond;
    public Stat AttackDamage;
    public Stat AttackArc;
    public Stat AttackRange;

    public float AttackSpeed { get => 1 / AttacksPerSecond.Value; }
}
