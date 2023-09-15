using Kryz.CharacterStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBaseSO : ScriptableObject
{
    [Header("Base Fields")]
    public GameObject UIWeaponIcon;
    public Sprite UIWeaponSprite;
    public bool AimAssist = false;
    public LayerMask EnemyLayer;
    public string WeaponName;

    public Stat AttacksPerSecond;
    public Stat AttackDamage;
    public Stat AttackArc;
    public Stat AttackRange;

    public float AttackSpeed { get => 1 / AttacksPerSecond.Value; }
}
