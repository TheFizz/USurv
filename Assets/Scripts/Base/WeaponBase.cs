
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class WeaponBase : MonoBehaviour
{
    public event OverlayFillHandler OnAbilityFillChanged;
    public event OverlayFillHandler OnHeatFillChanged;
    public event WeaponOverheatHandler OnWeaponOverheated;

    [SerializeField] private WeaponBaseSO _defaultWeaponData;
    [SerializeField] private AbilityBase _defaultWeaponAbility;

    [HideInInspector] public WeaponBaseSO WeaponData;
    [HideInInspector] public AbilityBase WeaponAbility;

    private GameObject _uiObj;
    [HideInInspector] public GameObject UIObject { get => GetUIObject(); }
    [HideInInspector] public RectTransform UIRect;

    [HideInInspector] public WeaponIcon UIWeaponIcon;


    [HideInInspector] public int WeaponLevel = 0;

    protected Transform Source;

    private float _abilityCooldown;
    private AbilityState _abilityState;
    protected HeatStatus HeatStatus = HeatStatus.None;

    private float _maxHeat = 100;
    private float _curHeat = 0;
    private float _cdRate = 4;
    private float _heatRate = 8;
    private int _myPosition;

    protected virtual void Awake()
    {
        InstantiateSOs();
        ValidateModsAndAssignSource();

        gameObject.name = WeaponData.WeaponName;
        WeaponLevel = 0;
    }

    private GameObject GetUIObject()
    {
        if (_uiObj != null)
            return _uiObj;

        _abilityState = AbilityState.Ready;
        _uiObj = Instantiate(WeaponData.UIWeaponIcon);
        _uiObj.GetComponent<WeaponIcon>().SetPartner(this);
        _uiObj.name = WeaponData.WeaponName + "Icon";
        UIRect = UIObject.GetComponent<RectTransform>();

        return _uiObj;
    }

    private void ValidateModsAndAssignSource()
    {
        foreach (var mod in WeaponData.PassiveModifiers)
        {
            mod.Source = this;
            mod.ValidateOrder();
        }
        foreach (var upgrade in WeaponData.UpgradePath)
        {
            foreach (var mod in upgrade.AbilityStatMods)
            {
                mod.Source = this;
                mod.ValidateOrder();
            }
            foreach (var mod in upgrade.PassiveStatMods)
            {
                mod.Source = this;
                mod.ValidateOrder();
            }
            foreach (var mod in upgrade.SelfStatMods)
            {
                mod.Source = this;
                mod.ValidateOrder();
            }

        }
    }

    private void InstantiateSOs()
    {
        WeaponData = Instantiate(_defaultWeaponData);
        WeaponAbility = Instantiate(_defaultWeaponAbility);
    }

    protected virtual void Update()
    {
        HandleAbilityCooldown();

        if (HeatStatus == HeatStatus.Overheated)
            return;
        _curHeat -= (Time.deltaTime * _cdRate);
        OnHeatFillChanged?.Invoke(_curHeat / _maxHeat);
        if (_curHeat <= 0)
        {
            _curHeat = 0;
            HeatStatus = HeatStatus.None;
            OnWeaponOverheated?.Invoke(false);
        }
    }

    public void SwappedTo(int position)
    {
        if (_myPosition == 0 && HeatStatus == HeatStatus.Overheated)
            HeatStatus = HeatStatus.Cooling;
        _myPosition = position;

    }
    public virtual void UseAbility()
    {
        if (_abilityState != AbilityState.Ready)
        {
            Debug.Log("Ability not ready (" + _abilityCooldown + ")");
            return;
        }
        WeaponAbility.Use(Source);
        _abilityState = AbilityState.Cooldown;
        var cd = WeaponAbility.AbilityCooldown;
        _abilityCooldown = cd - (cd * (WeaponAbility.GetStat(StatParam.CooldownReductionPerc).Value / 100));
    }
    protected virtual void Attack()
    {
        Globals.PSystems.OnWeaponAttack(WeaponData.GetStat(StatParam.AttackRange).Value, WeaponData.GetStat(StatParam.AttackCone).Value);
        AlignAttackVector();
    }
    public virtual void StartAttack()
    {
        if (Source == null)
            Source = Globals.PlayerTransform.Find("AttackSource").gameObject.transform;
        Source.rotation = Source.parent.rotation;
        InvokeRepeating("Attack", 1, WeaponData.AttackSpeed);
    }

    public virtual void RestartAttack()
    {
        CancelInvoke();
        InvokeRepeating("Attack", 0, WeaponData.AttackSpeed);
    }

    public virtual void StopAttack()
    {
        CancelInvoke();
    }

    #region Private methods 

    protected void AddHeat(float heat)
    {
        if (HeatStatus == HeatStatus.Overheated)
            return;

        heat *= _heatRate;
        _curHeat += heat;
        OnHeatFillChanged?.Invoke(_curHeat / _maxHeat);
        if (_curHeat >= _maxHeat)
        {
            _curHeat = _maxHeat;
            HeatStatus = HeatStatus.Overheated;
            OnWeaponOverheated?.Invoke(true);
        }
    }
    private void AlignAttackVector()
    {
        if (!WeaponData.AimAssist)
            return;
        Ray ray = Globals.MainCamera.ScreenPointToRay(Globals.InputHandler.MousePosition);
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
            WeaponAbility.GetStat(mod.Param)?.AddModifier(mod);
        }
    }
    public virtual void ClearSourcedModifiers(object source)
    {
        foreach (var stat in WeaponData.Stats)
        {
            stat.RemoveAllModifiersFromSource(source);
        }
    }
    private void HandleAbilityCooldown()
    {

        var cd = WeaponAbility.AbilityCooldown;
        var cdr = WeaponAbility.GetStat(StatParam.CooldownReductionPerc).Value;

        if (_abilityState == AbilityState.Cooldown)
        {
            OnAbilityFillChanged?.Invoke(_abilityCooldown / (cd - (cd * (cdr / 100))));

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
            stat.AddModifier(upgradeMod);
        }

        foreach (var upgradeMod in upgrade.AbilityStatMods)
        {
            var stat = WeaponAbility.GetStat(upgradeMod.Param);
            if (stat == null)
                continue;
            stat.AddModifier(upgradeMod);
        }

        foreach (var upgradeMod in upgrade.PassiveStatMods)
        {
            var passiveMod = WeaponData.PassiveModifiers.Find(x => x.Param == upgradeMod.Param);
            WeaponData.PassiveModifiers.Add(upgradeMod);
        }
        WeaponLevel++;
    }
    public virtual void UpgradeToLevelOld(int level)
    {
        var upgrade = WeaponData.UpgradePath.Find(x => x.UpgradeNumber == level);

        foreach (var upgradeMod in upgrade.SelfStatMods)
        {
            var stat = WeaponData.GetStat(upgradeMod.Param);

            if (stat == null)
                continue;

            //var statMods = stat.GetStatModifiersFromSource(this);
            //if (statMods.Count > 0)
            //    foreach (var statMod in statMods)
            //    {
            //        if (statMod.Type == upgradeMod.Type)
            //        {
            //            stat.RemoveModifier(statMod);
            //            statMod.CombineWith(upgradeMod);
            //            stat.AddModifier(statMod);
            //        }
            //        else
            //            stat.AddModifier(upgradeMod);
            //    }
            //else
            stat.AddModifier(upgradeMod);
        }

        foreach (var upgradeMod in upgrade.AbilityStatMods)
        {
            var stat = WeaponAbility.GetStat(upgradeMod.Param);

            if (stat == null)
                continue;

            //var statMods = stat.GetStatModifiersFromSource(this);
            //if (statMods.Count > 0)
            //    foreach (var statMod in statMods)
            //    {
            //        if (statMod.Type == upgradeMod.Type)
            //        {
            //            stat.RemoveModifier(statMod);
            //            statMod.CombineWith(upgradeMod);
            //            stat.AddModifier(statMod);
            //        }
            //        else
            //            stat.AddModifier(upgradeMod);
            //    }
            //else
            stat.AddModifier(upgradeMod);
        }

        foreach (var upgradeMod in upgrade.PassiveStatMods)
        {
            var passiveMod = WeaponData.PassiveModifiers.Find(x => x.Param == upgradeMod.Param);

            WeaponData.PassiveModifiers.Add(upgradeMod);

            //if (passiveMod == null)
            //    WeaponData.PassiveModifiers.Add(upgradeMod);
            //else
            //{
            //    passiveMod.CombineWith(upgradeMod);

            //}
        }
        WeaponLevel++;
    }
}
