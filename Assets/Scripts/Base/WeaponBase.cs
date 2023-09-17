using Kryz.CharacterStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class WeaponBase : MonoBehaviour
{
    public abstract List<StatModifier> WeaponModifiers { get; }
    [HideInInspector] public GameObject UIObject;
    [HideInInspector] public Image UIImage;
    [HideInInspector] public Image UIOverlay;
    [HideInInspector] public RectTransform UIRect;

    public WeaponBaseSO WeaponData;
    public AbilityBase WeaponAbility;

    protected HeatSystem Heat;
    protected Transform Source;

    private PlayerDamageHandler _pDamageHandler;
    private InputHandler _input;
    public float AbilityCooldown;
    public AbilityState AbilityState;
    protected StatModifierTracker _statModifierTracker;

    protected virtual void Awake()
    {
        Heat = Globals.Heat;
        AbilityState = AbilityState.Ready;
        UIObject = Instantiate(WeaponData.UIWeaponIcon);
        UIObject.name = WeaponData.WeaponName + "Icon";
        UIRect = UIObject.GetComponent<RectTransform>();
        UIImage = UIObject.GetComponent<Image>();
        UIImage.sprite = WeaponData.UIWeaponSprite;

        var overlayObj = UIObject.transform.GetChild(0);
        UIOverlay = overlayObj.GetComponentInChildren<Image>();

        _input = Globals.Input;
        _statModifierTracker = Globals.StatModTracker;
        _pDamageHandler = Globals.PlayerTransform.GetComponent<PlayerDamageHandler>();

    }
    protected virtual void Update()
    {
        HandleAbilityCooldown();
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
            _pDamageHandler.Damage(WeaponData.AttackDamage.Value, WeaponData.WeaponName, true);
        }
        AlignAttackVector();
    }
    public virtual void StartAttack()
    {
        Source.rotation = Source.parent.rotation;
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
        Ray ray = Globals.MainCamera.ScreenPointToRay(_input.MousePosition);
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
                case StatParam.AttackSpeed:
                    WeaponData.AttacksPerSecond.AddModifier(mod);
                    break;
                case StatParam.AttackDamage:
                    WeaponData.AttackDamage.AddModifier(mod);
                    break;
                case StatParam.AttackCone:
                    WeaponData.AttackCone.AddModifier(mod);
                    break;
                case StatParam.AttackRange:
                    WeaponData.AttackRange.AddModifier(mod);
                    break;
                default:
                    break;
            }

        }

    }
    public virtual void ClearModifiers()
    {
        WeaponData.AttackCone.RemoveAllModifiers();
        WeaponData.AttackDamage.RemoveAllModifiers();
        WeaponData.AttackRange.RemoveAllModifiers();
        WeaponData.AttacksPerSecond.RemoveAllModifiers();

    }
    private void HandleAbilityCooldown()
    {
        if (AbilityState == AbilityState.Cooldown)
        {
            UIOverlay.fillAmount = AbilityCooldown / WeaponAbility.AbilityCooldown;

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
        return WeaponData.UIWeaponSprite;
    }
    public void SetSource(Transform source)
    {
        Source = source;
    }
    #endregion
}
