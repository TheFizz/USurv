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
    public GameObject ArrowModel;
    public ParticleSystem Particles;
    private bool _canMove = true;
    private WeaponBaseRanged _weapon;
    public void Setup(WeaponBaseRanged weapon, Transform source)
    {
        WeaponRangedSO weaponData = (WeaponRangedSO)weapon.WeaponData;
        _weapon = weapon;
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
        if (Particles == null)
        {
            Destroy(gameObject);
            return;
        }

        if (_canMove)
        {
            transform.position += transform.forward * Time.deltaTime * _projectileSpeed;
        }
        var curDist = Vector3.Distance(transform.position, _sourcePoint);
        if (curDist >= _maxDistance)
        {
            Destroy(ArrowModel);
            Particles?.Stop();
            _canMove = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag != "Enemy")
            return;

        bool isCrit = false;
        bool disableProcs = false;
        int roll = Random.Range(0, 100);
        float chance = _critChance;
        float dmg = _attackDamage;
        if (roll < chance)
        {
            dmg *= (_critMult + Game.BaseCritMultiplierPerc) / 100;
            isCrit = true;
        }

        var enemy = other.GetComponent<NewEnemyBase>();

        if (enemy.Plating.PlatingType != PlatingType.None)
        {
            if (_weapon.Plating.PlatingType == enemy.Plating.PlatingType)
            {
                dmg *= 1.25f;
            }
            if (_weapon.Plating.PlatingType != enemy.Plating.PlatingType)
            {
                dmg *= 0.75f;
                disableProcs = true;
            }
        }

        enemy.Damage(dmg, isCrit);
        if (!disableProcs)
            if (Game.PSystems.CurrentTrinkets != null)
                foreach (var trinket in Game.PSystems.CurrentTrinkets)
                {
                    if (trinket is OnHitEffectTrinketSO)
                    {
                        var OHTrinket = (OnHitEffectTrinketSO)trinket;
                        OHTrinket.OnHitAction(enemy);
                    }
                    if (trinket is OnHitAttackTrinketSO)
                    {
                        var OHTrinket = (OnHitAttackTrinketSO)trinket;
                        OHTrinket.OnHitAttack();
                    }
                    if (trinket is OnHitStackTimedTrinketSO)
                    {
                        var OHTrinket = (OnHitStackTimedTrinketSO)trinket;
                        Game.PSystems.AddTimedTrinket(OHTrinket.Init());
                    }
                }
        if (_pierceCount <= 0)
        {
            Destroy(ArrowModel);
            Particles?.Stop();
            _canMove = false;
        }
        _pierceCount--;
    }
}
