using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    private float _projectileSpeed;
    private float _maxDistance;
    private Vector3 _sourcePoint;
    private int _pierceCount;
    private float _attackDamage;
    private float _critChance;
    private float _critMult;
    public void Setup(WeaponBaseRanged weapon, Transform source)
    {
        WeaponRangedSO weaponData = (WeaponRangedSO)weapon.WeaponData;

        _projectileSpeed = weaponData.GetStat(StatParam.ProjectileSpeed).Value;
        _pierceCount = Mathf.RoundToInt(weaponData.GetStat(StatParam.PierceCount).Value);
        _maxDistance = weaponData.GetStat(StatParam.AttackRange).Value;
        _attackDamage = weaponData.GetStat(StatParam.AttackDamage).Value;
        _sourcePoint = source.position;
        _critChance = weaponData.GetStat(StatParam.CritChancePerc).Value;
        _critMult = weaponData.GetStat(StatParam.CritMultiplierPerc).Value;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * _projectileSpeed;
        var curDist = Vector3.Distance(transform.position, _sourcePoint);
        if (curDist >= _maxDistance)
            Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Enemy")
            return;


        bool isCrit = false;
        int roll = Random.Range(0, 100);
        float chance = _critChance;
        float dmg = _attackDamage;
        if (roll < chance)
        {
            dmg *= (_critMult + Globals.BaseCritMultiplierPerc) / 100;
            isCrit = true;
        }

        var enemy = other.GetComponent<NewEnemyBase>();
        enemy.Damage(dmg, isCrit);
        if (_pierceCount <= 0)
            Destroy(gameObject);
        _pierceCount--;
    }
}
