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
    public void Setup(float projectileSpeed, int pierceCount, float maxDistance, float attackDamage, Vector3 sourcePoint)
    {
        _projectileSpeed = projectileSpeed;
        _pierceCount = pierceCount;
        _maxDistance = maxDistance;
        _attackDamage = attackDamage;
        _sourcePoint = sourcePoint;
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

        var enemy = other.GetComponent<EnemyBase>();
        enemy.Damage(_attackDamage);
        if (_pierceCount <= 0)
            Destroy(gameObject);
        _pierceCount--;
    }
}
