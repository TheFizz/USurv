using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKama : IWeapon
{
    public float AttackSpeed { get; set; } = 1f;
    public float AttackDamage { get; set; } = 1f;
    public float AttackArc { get; set; } = 30f;
    public float AttackRange { get; set; } = 2.5f;
}
