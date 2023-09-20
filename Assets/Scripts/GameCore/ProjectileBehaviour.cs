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
    public void Setup(float projectileSpeed, int pierceCount, float maxDistance, float attackDamage, float critChance, float critMult, Vector3 sourcePoint)
    {
        _projectileSpeed = projectileSpeed;
        _pierceCount = pierceCount;
        _maxDistance = maxDistance;
        _attackDamage = attackDamage;
        _sourcePoint = sourcePoint;
        _critChance = critChance;
        _critMult = critMult;
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
