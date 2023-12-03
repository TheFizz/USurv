
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBaseSO : ScriptableObject
{
    [Header("Base Fields")]
    public GameObject PickupObject;
    public GameObject UIWeaponIcon;
    public Sprite UIWeaponSprite;
    public bool AimAssist = false;
    public LayerMask EnemyLayer;
    public string WeaponName;
    public string AttackDescription;

    public List<Stat> Stats = new List<Stat>();
    public List<StatModifier> PassiveModifiers = new List<StatModifier>();
    //[SerializeField] public List<WeaponUpgradeSO> UpgradePath;
    [SerializeField] public List<WeaponUpgrade> UpgradePath;
    public float AttackSpeed { get => 1 / GetStat(StatParam.AttackSpeed).Value; }

    public Stat GetStat(StatParam param)
    {
        return Stats.Find(x => x.Parameter == param);
    }
}
