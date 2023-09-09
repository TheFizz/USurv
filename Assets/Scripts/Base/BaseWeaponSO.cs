using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName ="New Melee", menuName ="Weapons/Melee")]
public class BaseWeaponSO : ScriptableObject
{
    [Header("Weapon Fields")]

    public LayerMask enemyLayer;
    public float attackSpeed;
    public float attackDamage;
    public float attackArc;
    public float attackRange;
}
