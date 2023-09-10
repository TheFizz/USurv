using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpear : MonoBehaviour, IWeapon
{
    public BaseWeaponSO WeaponData;
    private Transform _source;
    public GameObject DamageArc;
    private InputHandler _input;
    void Awake()
    {
        var ps = GameObject.FindGameObjectWithTag("PeristentSystems");
        if (ps != null)
            _input = ps.GetComponent<InputHandler>();
    }

    void Update()
    {
        if (WeaponData.aimAssist)
            AlignAttackVector();
    }

    public void Attack()
    {

        var tmpRotation = _source.rotation;
        AlignAttackVector();

        var go = Instantiate(DamageArc, _source.position, _source.rotation, _source);
        go.transform.Rotate(90f, 0, 0);

        var forward = _source.forward;
        var arcValue = Mathf.Cos((WeaponData.attackArc / 2) * Mathf.Deg2Rad);
        var sourceFloored = new Vector3(_source.position.x, 0, _source.position.z);

        Collider[] hitEnemies = Physics.OverlapSphere(sourceFloored, WeaponData.attackRange, WeaponData.enemyLayer);
        foreach (var hitEnemy in hitEnemies)
        {
            Vector3 enemyPos = hitEnemy.transform.position;
            Vector3 enemyPosFloored = new Vector3(enemyPos.x, 0, enemyPos.z);
            Vector3 vectorToCollider = (enemyPosFloored - sourceFloored).normalized;
            var dot = Vector3.Dot(vectorToCollider, forward); //1 = right in front, -1 = right behind
            if (hitEnemy.name == "tpoint")
                Debug.Log(dot);
            if (dot >= arcValue)
            {
                var enemy = hitEnemy.GetComponent<EnemyBase>();
                enemy.Damage(WeaponData.attackDamage);
            }
        }

        _source.rotation = tmpRotation;
    }
    public void StartAttack(Transform source)
    {
        _source = source;
        InvokeRepeating("Attack", 1, WeaponData.AttackSpeed);
    }

    public void StopAttack()
    {
        CancelInvoke();
    }
    public void AlignAttackVector()
    {
        Ray ray = Camera.main.ScreenPointToRay(_input.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitinfo, layerMask: WeaponData.enemyLayer, maxDistance: 300f))
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
