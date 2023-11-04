using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviourEnemy : MonoBehaviour
{
    private float _projectileSpeed;
    private float _maxDistance;
    private Vector3 _sourcePoint;
    private float _attackDamage;
    private string _id;
    public void Setup(string ID, float speed, float distance, Vector3 source)
    {
        _id = ID;
        _projectileSpeed = speed;
        _maxDistance = distance;
        _attackDamage = 1;
        _sourcePoint = source;
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
        if (other.gameObject.tag != "Player")
            return;

        float dmg = _attackDamage;
        var enemy = other.GetComponent<PlayerDamageManager>();
        enemy.Damage(dmg, false, _id);
        Destroy(gameObject);
    }
}
