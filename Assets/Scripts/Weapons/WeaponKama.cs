using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKama : MonoBehaviour, IWeapon
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
    public void Attack()
    {

        var forward = _source.forward;
        var arcValue = (ConvertAngleToValue(WeaponData.attackArc / 2) * -1);
        var sourceFloored = new Vector3(_source.position.x, 0, _source.position.z);
        var error = 0.0001f;

        StartCoroutine(ShowMeleeAttack(.2f));

        Collider[] hitEnemies = Physics.OverlapSphere(sourceFloored, WeaponData.attackRange, WeaponData.enemyLayer);
        foreach (var hitEnemy in hitEnemies)
        {
            Vector3 enemyPos = hitEnemy.transform.position;
            Vector3 enemyPosFloored = new Vector3(enemyPos.x, 0, enemyPos.z);
            Vector3 vectorToCollider = (enemyPosFloored - sourceFloored).normalized;
            var dot = Vector3.Dot(vectorToCollider, forward) + error; //1 = right in front, -1 = right behind
            if (hitEnemy.name == "tpoint")
                Debug.Log(dot);
            if (dot >= arcValue)
            {
                var enemy = hitEnemy.GetComponent<EnemyBase>();
                enemy.Damage(WeaponData.attackDamage);
            }
        }
    }

    private IEnumerator ShowMeleeAttack(float time)
    {
        var go = Instantiate(DamageArc, _source.position, Quaternion.Euler(new Vector3(_source.rotation.eulerAngles.x + 90f, _source.rotation.eulerAngles.y-(WeaponData.attackArc/2), _source.rotation.eulerAngles.z)),_source);
        yield return new WaitForSeconds(time);
        Destroy(go);
    }

    public void StartAttack(Transform source)
    {
        _source = source;
        InvokeRepeating("Attack", 1, WeaponData.attackSpeed);
    }

    public void StopAttack()
    {
        CancelInvoke();
    }
    public void NormalizeAttackVector()
    {
        Ray ray = Camera.main.ScreenPointToRay(_input.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitinfo, layerMask: WeaponData.enemyLayer, maxDistance: 300f))
        {
            var targetLook = hitinfo.collider.transform.position;
            targetLook.y = _source.position.y;
            _source.LookAt(targetLook);
        }
    }
    private static float ConvertAngleToValue(float degrees)
    {
        degrees %= 360f;
        if (degrees > 180f)
            degrees = 360f - degrees;
        float value = (degrees / 180f) * 2f - 1f;
        return value;
    }
}
