using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    protected InputHandler _input;
    protected Transform _source;

    [SerializeField] protected WeaponBaseSO _weaponData;
    [SerializeField] protected GameObject _damageArc;

    protected virtual void Awake()
    {
        var ps = GameObject.FindGameObjectWithTag("PeristentSystems");
        if (ps != null)
            _input = ps.GetComponent<InputHandler>();
    }
    protected virtual void Update()
    {
        if (_weaponData.aimAssist)
            AlignAttackVector();
    }
    protected virtual void Attack()
    {
        var go = Instantiate(_damageArc, _source.position, _source.rotation, _source);
        go.transform.Rotate(90f, 0, 0);
        var forward = _source.forward;
        var arcValue = Mathf.Cos((_weaponData.attackArc / 2) * Mathf.Deg2Rad);
        var sourceFloored = new Vector3(_source.position.x, 0, _source.position.z);

        Collider[] hitEnemies = Physics.OverlapSphere(sourceFloored, _weaponData.attackRange, _weaponData.enemyLayer);
        foreach (var hitEnemy in hitEnemies)
        {
            Vector3 enemyPos = hitEnemy.transform.position;
            Vector3 enemyPosFloored = new Vector3(enemyPos.x, 0, enemyPos.z);
            Vector3 vectorToCollider = (enemyPosFloored - sourceFloored).normalized;
            var a = enemyPos.x;
            var dot = Vector3.Dot(vectorToCollider, forward); //1 = right in front, -1 = right behind
            if (hitEnemy.name == "tpoint")
            {
                Debug.Log("Cur: " + arcValue);

                Debug.Log("Target:" + dot);
            }
            if (dot >= arcValue)
            {
                var enemy = hitEnemy.GetComponent<EnemyBase>();
                enemy.Damage(_weaponData.attackDamage);
            }
        }
    }

    public virtual void StartAttack(Transform source)
    {
        _source = source;
        InvokeRepeating("Attack", 1, _weaponData.AttackSpeed);
    }

    public virtual void StopAttack()
    {
        CancelInvoke();
    }
    protected virtual void AlignAttackVector()
    {
        Ray ray = Camera.main.ScreenPointToRay(_input.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitinfo, layerMask: _weaponData.enemyLayer, maxDistance: 300f))
        {
            var targetLook = hitinfo.collider.transform.position;
            targetLook.y = _source.position.y;
            _source.LookAt(targetLook);
            Debug.DrawLine(_source.position, _source.forward * 100, Color.blue);
        }
        else
        {
            _source.rotation = _source.parent.rotation;
        }
    }
}
