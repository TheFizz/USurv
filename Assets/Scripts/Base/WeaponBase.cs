
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] private WeaponBaseSO _defaultWeaponData;
    [SerializeField] private AbilityBase _defaultWeaponAbility;

    [HideInInspector] public WeaponBaseSO WeaponData;
    [HideInInspector] public AbilityBase WeaponAbility;

    [HideInInspector] public GameObject UIObject;
    [HideInInspector] public Image UIImage;
    [HideInInspector] public Image UIOverlay;
    [HideInInspector] public RectTransform UIRect;


    [HideInInspector] public int WeaponLevel = 0;

    protected HeatSystem Heat;
    protected Transform Source;

    private PlayerDamageHandler _pDamageHandler;
    private InputHandler _input;
    private float AbilityCooldown;
    private AbilityState AbilityState;
    private UIManager _ui;

    private bool _isActive = false;

    protected virtual void Awake()
    {
        InstantiateSOs();
        WeaponLevel = 0;
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
        _pDamageHandler = Globals.PlayerTransform.GetComponent<PlayerDamageHandler>();
        _ui = Globals.UIManager;
    }

    private void InstantiateSOs()
    {
        WeaponData = Instantiate(_defaultWeaponData);
        WeaponAbility = Instantiate(_defaultWeaponAbility);

        List<WeaponUpgradeSO> upgrades = new List<WeaponUpgradeSO>();
        foreach (var upgrade in _defaultWeaponData.UpgradePath)
        {
            //upgrades.Add(Instantiate(upgrade));
        }
        // WeaponData.UpgradePath = upgrades;
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
        foreach (var stat in WeaponAbility.Stats)
        {
            text += $"ABL {stat.Parameter}: {stat.Value}\n";
            foreach (var mod in stat.StatModifiers)
            {
                text += $"ABL # {mod}\n";
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
            //Debug.Log($"Applied {mod} to {WeaponData.WeaponName}");
        }
    }
    public virtual void ClearLocalModifiers()
    {
        foreach (var stat in WeaponData.Stats)
        {
            stat.RemoveAllModifiersFromSource("LOCAL");
        }
        //Debug.Log($"Removed local mods from {WeaponData.WeaponName}");
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
    public virtual void UpgradeToLevel(int level)
    {
        var upgrade = WeaponData.UpgradePath.Find(x => x.UpgradeNumber == level);

        foreach (var upgradeMod in upgrade.SelfStatMods)
        {
            var stat = WeaponData.GetStat(upgradeMod.Param);

            if (stat == null)
                continue;

            var statMods = stat.GetStatModifiersFromSource("SELF");
            if (statMods.Count > 0)
                foreach (var statMod in statMods)
                {
                    if (statMod.Type == upgradeMod.Type)
                    {
                        stat.RemoveModifier(statMod);
                        statMod.CombineWith(upgradeMod);
                        stat.AddModifier(statMod);
                    }
                    else
                        stat.AddModifier(upgradeMod);
                }
            else
                stat.AddModifier(upgradeMod);
        }

        foreach (var upgradeMod in upgrade.AbilityStatMods)
        {
            var stat = WeaponAbility.GetStat(upgradeMod.Param);

            if (stat == null)
                continue;

            var statMods = stat.GetStatModifiersFromSource("SELF");
            if (statMods.Count > 0)
                foreach (var statMod in statMods)
                {
                    if (statMod.Type == upgradeMod.Type)
                    {
                        stat.RemoveModifier(statMod);
                        statMod.CombineWith(upgradeMod);
                        stat.AddModifier(statMod);
                    }
                    else
                        stat.AddModifier(upgradeMod);
                }
            else
                stat.AddModifier(upgradeMod);
        }

        foreach (var upgradeMod in upgrade.PassiveStatMods)
        {
            upgradeMod.Source = "LOCAL";
            var passiveMod = WeaponData.PassiveModifiers.Find(x => x.Param == upgradeMod.Param);

            if (passiveMod == null)
                WeaponData.PassiveModifiers.Add(upgradeMod);
            else
            {
                passiveMod.CombineWith(upgradeMod);

            }
        }
        WeaponLevel++;
    }
}
