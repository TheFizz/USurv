using RobinGoodfellow.CircleGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyLayer;
    private IWeapon _weapon;
    public GameObject _attackPoint;
    // Start is called before the first frame update
    void Start()
    {
        _weapon = new WeaponKama();
        InvokeRepeating("DoAttack", 1, _weapon.AttackSpeed);

    }
    private void DoAttack()
    {
        if (_weapon == null)
            return;

        if (_weapon is IRanged)
        {
            AttackRanged(_attackPoint.transform.position);
        }
        else
        {
            AttackMelee(_attackPoint.transform.position);
        }

    }

    void AttackMelee(Vector3 source)
    {
        var forward = transform.forward;
        var arcValue = (ConvertAngleToValue(_weapon.AttackArc / 2) * -1);
        source = new Vector3(source.x, 0, source.z);

        Debug.Log(arcValue);
        Collider[] hitEnemies = Physics.OverlapSphere(source, _weapon.AttackRange, _enemyLayer);
        foreach (var hitEnemy in hitEnemies)
        {
            Vector3 enemyPos = hitEnemy.transform.position;
            Vector3 enemyPosFloored = new Vector3(enemyPos.x, 0, enemyPos.z);
            Vector3 vectorToCollider = (enemyPosFloored - source).normalized;
            var dot = Vector3.Dot(vectorToCollider, forward); //1 = right in front, -1 = right behind
            if (hitEnemy.name == "tpoint")
                Debug.Log(dot);
            if (dot >= arcValue)
            {
                var enemy = hitEnemy.GetComponent<EnemyBase>();
                enemy.Damage(_weapon.AttackDamage);
            }
        }
    }
    void AttackRanged(Vector3 source)
    {
        var projectile = (GameObject)Instantiate(((IRanged)_weapon).Projectile, _attackPoint.transform.position, _attackPoint.transform.rotation);
        var movScript = projectile.GetComponent<ProjectileMovement>();

        movScript.Setup(
            ((IRanged)_weapon).ProjectileSpeed,
            ((IRanged)_weapon).PierceCount,
            _weapon.AttackRange,
            _weapon.AttackDamage,
            source
            );
    }
    private void OnDrawGizmosSelected()
    {
        if (_weapon == null)
            return;
        if (_weapon is not IRanged)
            Gizmos.DrawWireSphere(_attackPoint.transform.position, _weapon.AttackRange);
    }
    public static float ConvertAngleToValue(float degrees)
    {
        // Ensure degrees are within the range of 0 to 360
        degrees %= 360f;

        // Normalize degrees to be within the range of 0 to 180
        if (degrees > 180f)
        {
            degrees = 360f - degrees;
        }

        // Calculate the value between 1 and -1 based on the normalized angle
        float value = (degrees / 180f) * 2f - 1f;

        return value;
    }
}
