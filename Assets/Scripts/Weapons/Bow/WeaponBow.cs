using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBow : MonoBehaviour, IWeapon
{
    public BaseRangedWeaponSO WeaponData;
    private Transform _source;
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
        if (gameObject.activeSelf == false)
        {
            Debug.LogWarning(name + " is not active! (Attack)");
            return;
        }



        var projectile = (GameObject)Instantiate(WeaponData.projectile, _source.position, _source.rotation);
        var movScript = projectile.GetComponent<ProjectileBehaviour>();

        movScript.Setup(
            WeaponData.projectileSpeed,
            WeaponData.pierceCount,
            WeaponData.attackRange,
            WeaponData.attackDamage,
            _source.position
            );

    }

    public void StartAttack(Transform source)
    {
        if (gameObject.activeSelf == false)
        {
            Debug.LogWarning(name + " is not active! (StartAttack)");
            return;
        }
        _source = source;
        InvokeRepeating("Attack", 1, WeaponData.AttackSpeed);

    }

    public void StopAttack()
    {
        if (gameObject.activeSelf == false)
        {
            Debug.LogWarning(name + " is not active! (StopAttack)");
            return;
        }
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
