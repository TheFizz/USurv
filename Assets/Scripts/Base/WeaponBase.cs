using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public abstract List<StatModifier> WeaponModifiers { get; }

    [SerializeField] protected WeaponBaseSO _weaponData;
    [SerializeField] protected AbilityBase _weaponAbility;
    protected WeaponBaseSO _weaponDataModified;

    protected InputHandler _input;
    protected Transform _source;
    private StatModifierTracker _statModifierTracker;

    private AbilityState _abilityState = AbilityState.Ready;
    private float _abilityCooldown;

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
        HandleAbilityCooldown();
        AlignAttackVector();
    }
    public virtual void UseAbility()
    {
        if (_abilityState != AbilityState.Ready)
        {
            Debug.Log("Ability not ready (" + _abilityCooldown + ")");
            return;
        }
        _weaponAbility.Use(_source);
        _abilityState = AbilityState.Cooldown;
        _abilityCooldown = _weaponAbility.AbilityCooldown;
    }
    protected abstract void Attack();
    public virtual void StartAttack()
    {
        _weaponDataModified = Instantiate(_weaponData);
        ApplyModifiers();
        InvokeRepeating("Attack", 1, _weaponDataModified.AttackSpeed);
    }
    public virtual void StopAttack()
    {
        CancelInvoke();
        Destroy(_weaponDataModified);
    }

    #region Private methods 
    private void AlignAttackVector()
    {
        if (_weaponDataModified == null)
            return;
        if (!_weaponDataModified.AimAssist)
            return;
        Ray ray = Camera.main.ScreenPointToRay(_input.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitinfo, layerMask: _weaponDataModified.EnemyLayer, maxDistance: 300f))
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
    private void ApplyModifiers()
    {
        foreach (var mod in _statModifierTracker.Modifiers)
        {
            switch (mod.Parameter)
            {
                case StatModParameter.AttackSpeed:
                    _weaponDataModified.AttacksPerSecond = CalculateModifier(_weaponDataModified.AttacksPerSecond, mod);
                    break;
                case StatModParameter.AttackDamage:
                    _weaponDataModified.AttackDamage = CalculateModifier(_weaponDataModified.AttackDamage, mod);
                    break;
                case StatModParameter.AttackArc:
                    _weaponDataModified.AttackArc = CalculateModifier(_weaponDataModified.AttackArc, mod);
                    break;
                case StatModParameter.AttackRange:
                    _weaponDataModified.AttackRange = CalculateModifier(_weaponDataModified.AttackRange, mod);
                    break;
                default:
                    break;
            }

        }

    }
    private float CalculateModifier(float baseVal, StatModifier mod)
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
    private void HandleAbilityCooldown()
    {

        if (_abilityState == AbilityState.Cooldown)
        {
            if (_abilityCooldown > 0)
                _abilityCooldown -= Time.deltaTime;
            else
                _abilityState = AbilityState.Ready;
        }
    }
    #endregion

    #region Public methods
    public Sprite GetWeaponImage()
    {
        return _weaponData.WeaponSprite;
    }
    public void SetSource(Transform source)
    {
        _source = source;
    }
    #endregion
}
