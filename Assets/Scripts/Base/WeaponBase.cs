using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    protected InputHandler _input;
    protected Transform _source;
    public abstract List<StatModifier> Modifiers { get; }

    [SerializeField] protected WeaponBaseSO _weaponData;
    [SerializeField] protected GameObject _damageArc;
    [SerializeField] protected WeaponBaseSO _weaponDataModified;
    private StatModifierTracker _statModifierTracker;
    protected virtual void Awake()
    {
        var ps = GameObject.FindGameObjectWithTag("PeristentSystems");
        if (ps != null)
        {
            _input = ps.GetComponent<InputHandler>();
            _statModifierTracker = ps.GetComponent<StatModifierTracker>();
        }
    }
    protected virtual void Update()
    {
        if (_weaponDataModified != null)
            if (_weaponDataModified.aimAssist)
                AlignAttackVector();
    }
    public virtual void UseAbility(Transform source)
    {
        if (_source == null)
            _source = source;
    }
    protected abstract void Attack();
    public virtual void StartAttack(Transform source)
    {
        _weaponDataModified = Instantiate(_weaponData);
        if (_source == null)
            _source = source;
        ApplyModifiers();
        InvokeRepeating("Attack", 1, _weaponDataModified.AttackSpeed);
    }
    public virtual void StopAttack()
    {
        CancelInvoke();
        Destroy(_weaponDataModified);
    }
    protected virtual void AlignAttackVector()
    {
        Ray ray = Camera.main.ScreenPointToRay(_input.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitinfo, layerMask: _weaponDataModified.enemyLayer, maxDistance: 300f))
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
    protected void ApplyModifiers()
    {
        foreach (var mod in _statModifierTracker.Modifiers)
        {
            switch (mod.Parameter)
            {
                case StatModParameter.AttackSpeed:
                    _weaponDataModified.attacksPerSecond = CalculateModifier(_weaponDataModified.attacksPerSecond, mod);
                    break;
                case StatModParameter.AttackDamage:
                    _weaponDataModified.attackDamage = CalculateModifier(_weaponDataModified.attackDamage, mod);
                    break;
                case StatModParameter.AttackArc:
                    _weaponDataModified.attackArc = CalculateModifier(_weaponDataModified.attackArc, mod);
                    break;
                case StatModParameter.AttackRange:
                    _weaponDataModified.attackRange = CalculateModifier(_weaponDataModified.attackRange, mod);
                    break;
                default:
                    break;
            }

        }

    }
    float CalculateModifier(float baseVal, StatModifier mod)
    {
        switch (mod.Type)
        {
            case StatModType.Flat:
                return baseVal += mod.Value;
            case StatModType.Percent:
                return baseVal *= 1f + (mod.Value / 100f);
            default:
                Debug.LogError(mod.Type + "is not recognized!");
                return -1;
        }
    }

    public Sprite GetWeaponImage()
    {
        return _weaponData.weaponSprite;
    }
}
