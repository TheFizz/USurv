
using System;
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
    private UIManager _ui;

    private bool _isActive = false;

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
        _ui = Globals.UIManager;

    }
    protected virtual void Update()
    {
        HandleAbilityCooldown();
        if (_isActive)
            ShowDebugText();
    }

    private void ShowDebugText()
    {
        string text = "";
        foreach (var stat in WeaponData.Stats)
        {
            text += $"{stat.Parameter}: {stat.Value}\n";
            foreach (var mod in stat.StatModifiers)
            {

                text += $"# {mod}\n";
            }
        }
        _ui.WriteDebug(text);
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
            _pDamageHandler.Damage(WeaponData.GetStat(StatParam.AttackDamage).Value, WeaponData.WeaponName, true);
        }
        AlignAttackVector();
    }
    public virtual void StartAttack()
    {
        Source.rotation = Source.parent.rotation;
        _isActive = true;
        InvokeRepeating("Attack", 1, WeaponData.AttackSpeed);
    }

    public virtual void RestartAttack()
    {
        CancelInvoke();
        InvokeRepeating("Attack", 0, WeaponData.AttackSpeed);
    }

    public virtual void StopAttack()
    {
        _isActive = false;
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
            WeaponData.GetStat(mod.Param)?.AddModifier(mod);
            Debug.Log($"Applied {mod} to {WeaponData.WeaponName}");
        }
    }
    public virtual void ClearLocalModifiers()
    {
        foreach (var stat in WeaponData.Stats)
        {
            stat.RemoveAllModifiersFromSource("LOCAL");
        }
        Debug.Log($"Removed local mods from {WeaponData.WeaponName}");
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
    private void OnDestroy()
    {
        foreach (var stat in WeaponData.Stats)
        {
            stat.RemoveAllModifiers();
        }
    }
}
