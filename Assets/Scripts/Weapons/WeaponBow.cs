using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBow : IWeapon, IRanged
{

    public float AttackSpeed { get; set; } = 1f;
    public float AttackDamage { get; set; } = 1f;
    public float AttackArc { get; set; } = 15f;
    public float AttackRange { get; set; } = 25f;
    public int PierceCount { get; set; } = 3;
    public float ProjectileSpeed { get; set; } = 25f;
    public Object Projectile { get; set; } = Resources.Load("Prefabs/Projectiles/Arrow");
}
