using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Ranged", menuName = "Weapons/Ranged")]
public class WeaponBaseRangedSO : WeaponBaseSO
{
    [Header("Ranged Fields")]
    public int pierceCount;
    public float projectileSpeed;
    public GameObject projectile;
}
