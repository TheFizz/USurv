using Kryz.CharacterStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public abstract List<StatModifier> WeaponModifiers { get; }
    public Transform UIIcon;
    [SerializeField] protected WeaponBaseSO WeaponData;
    [SerializeField] public AbilityBase WeaponAbility;

    protected HeatSystem Heat;
    protected Transform Source;
    //protected WeaponBaseSO WeaponDataModified;

    private PlayerStats _stats;
    private InputHandler _input;
    public float AbilityCooldown;
    public AbilityState AbilityState;
    private StatModifierTracker _statModifierTracker;

    protected virtual void Awake()
    {
        Heat = Globals.Heat;
        _input = Globals.Input;
        AbilityState = AbilityState.Ready;
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
        if (AbilityState != AbilityState.Ready)
        {
            Debug.Log("Ability not ready (" + AbilityCooldown + ")");
            return;
        }
        WeaponAbility.Use(Source);
        AbilityState = AbilityState.Cooldown;
        AbilityCooldown = WeaponAbility.AbilityCooldown;
    }
    protected virtual void Attack()
    {
        if (Heat.GetHeatStatus() == HeatStatus.Overheated)
        {
            _stats.Damage(WeaponData.AttackDamage.Value, true);
        }
    }
    public virtual void StartAttack()
    {
        ApplyModifiers(_statModifierTracker.LocalModifiers);
        InvokeRepeating("Attack", 1, WeaponData.AttackSpeed);
    }
    public virtual void StopAttack()
    {
        ClearModifiers();
        CancelInvoke();
    }

    #region Private methods 
    private void AlignAttackVector()
    {
        if (!WeaponData.AimAssist)
            return;
        Ray ray = Camera.main.ScreenPointToRay(_input.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitinfo, layerMask: WeaponData.EnemyLayer, maxDistance: 300f))
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
    public virtual void ApplyModifiers(List<StatModifier> mods)
    {
        foreach (var mod in mods)
        {
            switch (mod.Param)
            {
                case StatModParam.AttackSpeed:
                    WeaponData.AttacksPerSecond.AddModifier(mod);
                    break;
                case StatModParam.AttackDamage:
                    WeaponData.AttackDamage.AddModifier(mod);
                    break;
                case StatModParam.AttackArc:
                    WeaponData.AttackArc.AddModifier(mod);
                    break;
                case StatModParam.AttackRange:
                    WeaponData.AttackRange.AddModifier(mod);
                    break;
                default:
                    break;
            }

        }

    }
    public virtual void ClearModifiers()
    {
        WeaponData.AttackArc.RemoveAllModifiers();
        WeaponData.AttackDamage.RemoveAllModifiers();
        WeaponData.AttackRange.RemoveAllModifiers();
        WeaponData.AttacksPerSecond.RemoveAllModifiers();
    }
    private void HandleAbilityCooldown()
    {
        if (AbilityState == AbilityState.Cooldown)
        {
            if (AbilityCooldown > 0)
                AbilityCooldown -= Time.deltaTime;
            else
                AbilityState = AbilityState.Ready;
        }
    }
    #endregion

    #region Public methods
    public Sprite GetWeaponImage()
    {
        return WeaponData.WeaponSprite;
    }
    public void SetSource(Transform source)
    {
        Source = source;
    }
    #endregion
}
