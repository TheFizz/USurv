using Kryz.CharacterStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Ranged", menuName = "Weapons/Ranged")]
public class WeaponRangedSO : WeaponBaseSO
{
    [Header("Ranged Fields")]

    public Stat PierceCount;
    public Stat ProjectileSpeed;
    public GameObject Projectile;
}
