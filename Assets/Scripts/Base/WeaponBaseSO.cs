using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee", menuName = "Weapons/Melee")]
public class WeaponBaseSO : ScriptableObject
{
    [Header("Weapon Fields")]
    public bool aimAssist = false;

    public LayerMask enemyLayer;
    public float attacksPerSecond;
    public float attackDamage;
    public float attackArc;
    public float attackRange;
    public float AttackSpeed { get => 1 / attacksPerSecond; }
}
