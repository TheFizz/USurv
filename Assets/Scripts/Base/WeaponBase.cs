
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class WeaponBase : MonoBehaviour
{
    public event OverlayFillHandler OnAbilityFillChanged;

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

    private int _myPosition;

    public Plating Plating;

    Coroutine attack;

    public List<TrinketSO> PassiveTrinkets;

    protected virtual void Awake()
    {
        InstantiateSOs();
        ValidateModsAndAssignSource();
        gameObject.name = WeaponData.WeaponName;
        WeaponLevel = 0;

        Plating.PlatingType = PlatingType.Jade;
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

    }

    public void SwappedTo(int position)
    {
        _myPosition = position;
    }
    public virtual void UseAbility()
    {
        if (_abilityState != AbilityState.Ready)
            return;
        WeaponAbility.Use(Source);
        _abilityState = AbilityState.Cooldown;
        var cd = WeaponAbility.AbilityCooldown;
        _abilityCooldown = cd - (cd * (WeaponAbility.GetStat(StatParam.CooldownReductionPerc).Value / 100));
    }
    public void AttackAfterDelay(float delay, List<TrinketSO> trinkets = null)
    {
        StartCoroutine(AttackAfterDelayCR(delay, trinkets));
    }
    IEnumerator AttackAfterDelayCR(float delay, List<TrinketSO> trinkets)
    {
        yield return new WaitForSeconds(delay);
        Attack();
    }
    protected virtual void Attack()
    {
        Game.PSystems.OnWeaponAttack(WeaponData.GetStat(StatParam.AttackRange).Value, WeaponData.GetStat(StatParam.AttackCone).Value);
        AlignAttackVector();

    }
    public virtual void StartAttack()
    {
        if (Game.Room.State != RoomState.Active)
            return;
        if (Source == null)
            Source = Game.PSystems.AttackSource;
        Source.rotation = Source.parent.rotation;
        //InvokeRepeating("Attack", 1, WeaponData.AttackSpeed);
        if (attack == null)
            attack = StartCoroutine(AttackCR());
    }
    IEnumerator AttackCR()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            Attack();
            yield return new WaitForSeconds(WeaponData.AttackSpeed);
        }

    }
    public virtual void StopAttack()
    {
        //CancelInvoke();
        if (attack != null)
        {
            StopCoroutine(attack);
            attack = null;
        }
    }

    #region Private methods 

    private void AlignAttackVector()
    {
        if (!WeaponData.AimAssist)
            return;
        Ray ray = Game.MainCamera.ScreenPointToRay(Game.InputHandler.MousePosition);
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
