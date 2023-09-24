
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

    protected Transform Source;

    private float AbilityCooldown;
    private AbilityState AbilityState;

    protected virtual void Awake()
    {
        InstantiateSOs();
        ValidateModsAndAssignSource();
        gameObject.name = WeaponData.WeaponName;
        WeaponLevel = 0;
        AbilityState = AbilityState.Ready;
        UIObject = Instantiate(WeaponData.UIWeaponIcon);
        UIObject.name = WeaponData.WeaponName + "Icon";
        UIRect = UIObject.GetComponent<RectTransform>();
        UIImage = UIObject.GetComponent<Image>();
        UIImage.sprite = WeaponData.UIWeaponSprite;

        var overlayObj = UIObject.transform.GetChild(0);
        UIOverlay = overlayObj.GetComponentInChildren<Image>();
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
        AlignAttackVector();
    }
    public virtual void StartAttack()
    {
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
