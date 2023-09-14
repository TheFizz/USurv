using Kryz.CharacterStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public abstract List<StatModifier> WeaponModifiers { get; }
    [SerializeField] private WeaponBaseSO _weaponData;
    [SerializeField] private AbilityBase _weaponAbility;

    protected HeatSystem Heat;
    protected Transform Source;
    protected WeaponBaseSO WeaponDataModified;

    private PlayerStats _stats;
    private InputHandler _input;
    private float _abilityCooldown;
    private AbilityState _abilityState;
    private StatModifierTracker _statModifierTracker;

    protected virtual void Awake()
    {
        Heat = Globals.Heat;
        _input = Globals.Input;
        _abilityState = AbilityState.Ready;
        _statModifierTracker = Globals.StatModTracker;
        _stats = Globals.PlayerTransform.GetComponent<PlayerStats>();
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
        _weaponAbility.Use(Source);
        _abilityState = AbilityState.Cooldown;
        _abilityCooldown = _weaponAbility.AbilityCooldown;
    }
    protected virtual void Attack()
    {
        if (Heat.GetHeatStatus() == HeatStatus.Overheated)
        {
            _stats.Damage(WeaponDataModified.AttackDamage, true);
        }
    }
    public virtual void StartAttack()
    {
        WeaponDataModified = Instantiate(_weaponData);
        ApplyModifiers();
        InvokeRepeating("Attack", 1, WeaponDataModified.AttackSpeed);
    }
    public virtual void StopAttack()
    {
        CancelInvoke();
        Destroy(WeaponDataModified);
    }

    #region Private methods 
    private void AlignAttackVector()
    {
        if (WeaponDataModified == null)
            return;
        if (!WeaponDataModified.AimAssist)
            return;
        Ray ray = Camera.main.ScreenPointToRay(_input.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitinfo, layerMask: WeaponDataModified.EnemyLayer, maxDistance: 300f))
        {
            var targetLook = hitinfo.collider.transform.position;
            targetLook.y = Source.position.y;
            Source.LookAt(targetLook);
        }
        else
        {
            Source.rotation = Source.parent.rotation;
        }
    }
    private void ApplyModifiers()
    {
        foreach (var mod in _statModifierTracker.Modifiers)
        {
            switch (mod.Parameter)
            {
                case StatModParameter.AttackSpeed:
                    WeaponDataModified.AttacksPerSecond = CalculateModifier(WeaponDataModified.AttacksPerSecond, mod);
                    break;
                case StatModParameter.AttackDamage:
                    WeaponDataModified.AttackDamage = CalculateModifier(WeaponDataModified.AttackDamage, mod);
                    break;
                case StatModParameter.AttackArc:
                    WeaponDataModified.AttackArc = CalculateModifier(WeaponDataModified.AttackArc, mod);
                    break;
                case StatModParameter.AttackRange:
                    WeaponDataModified.AttackRange = CalculateModifier(WeaponDataModified.AttackRange, mod);
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
        Source = source;
    }
    #endregion
}
