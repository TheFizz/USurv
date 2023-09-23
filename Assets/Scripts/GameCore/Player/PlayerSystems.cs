
using RobinGoodfellow.CircleGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerSystems : MonoBehaviour
{
    static bool isNew = true;
    const int ACTIVE = 0;
    const int PASSIVE = 1;
    const int ABILITY = 2;

    [HideInInspector] public GlobalUpgradePathSO GlobalUpgradePath;
    [SerializeField] private GlobalUpgradePathSO _defaultGlobalUpgradePath;

    [HideInInspector] public PlayerStatsSO PlayerStats;
    [SerializeField] private PlayerStatsSO _defaultPlayerStats;

    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private GameObject _attackSource;

    [SerializeField] private GameObject[] _weaponQueue = new GameObject[3];
    [HideInInspector] public WeaponBase[] Weapons = new WeaponBase[3];

    [HideInInspector] public float CurrentXP = 0;
    private float _xpThreshold;
    private float _xpThresholdMultipltier;
    private int _upgradesPerLevel = 3;
    private int _currentLevel = 1;

    private List<StatModifier> _globalMods = new List<StatModifier>();
    private InputHandler _input;
    private HeatSystem _heat;
    private UIManager _uiManager;

    [SerializeField] private EndScreen _endScreen;

    // Start is called before the first frame update
    private void Awake()
    {
        if (!isNew)
            return;
        isNew = true;
        InstantiateSOs();
        ValidateModsAndAssignSource();
        _heat = Globals.Heat;
        _input = Globals.Input;
        _uiManager = Globals.UIManager;

        _xpThreshold = PlayerStats.XPThresholdBase;
        _xpThresholdMultipltier = PlayerStats.XPThresholdMultiplier;

        for (int i = 0; i < _weaponQueue.Length; i++)
        {
            var go = _weaponQueue[i] = Instantiate(_weaponQueue[i]);
            Weapons[i] = go.GetComponent<WeaponBase>();
        }
        _uiManager.SetupWeaponIcons(Weapons);

        SetWeaponsSource();
        Weapons[ACTIVE].ApplyModifiers(Weapons[PASSIVE].WeaponData.PassiveModifiers);
        Weapons[ACTIVE].StartAttack();
    }

    private void InstantiateSOs()
    {
        PlayerStats = Instantiate(_defaultPlayerStats);
        GlobalUpgradePath = Instantiate(_defaultGlobalUpgradePath);
    }
    private void Update()
    {
        string text = Weapons[ABILITY].WeaponAbility.AbilityName + "\n";
        foreach (var stat in Weapons[ABILITY].WeaponAbility.Stats)
        {
            text += $"{stat.Parameter}: {stat.Value}\n";
            foreach (var mod in stat.StatModifiers)
            {
                text += $"# {mod}\n";
            }
        }
        _uiManager.WriteDebugAbl(text);

        text = "Player\n";
        foreach (var stat in PlayerStats.Stats)
        {
            text += $"{stat.Parameter}: {stat.Value}\n";
            foreach (var mod in stat.StatModifiers)
            {
                text += $"# {mod}\n";
            }
        }
        _uiManager.WriteDebugPlr(text);


        if (Input.GetKeyDown("k"))
            Game.Instance.NextLevel();

        Debug.DrawRay(_attackSource.transform.position, _attackSource.transform.forward * 10, Color.green);

        if (_input.SwapWeapon && _heat.CanSwap())
            SwapRotate();

        if (_input.Swap01 && _heat.CanSwap())
            SwapSlots(0, 1);

        if (_input.Swap12 && _heat.CanSwap())
            SwapSlots(1, 2);

        if (_input.UseAbility)
            Weapons[ABILITY].UseAbility();
    }

    public void AddXP(float xpValue)
    {
        if (CurrentXP + xpValue < _xpThreshold)
            CurrentXP += xpValue;
        else
        {
            CurrentXP = xpValue - (_xpThreshold - CurrentXP);
            _xpThreshold *= _xpThresholdMultipltier;
            _currentLevel++;
            LevelUp();
        }
    }
    private void LevelUp()
    {
        List<StatParam> randomParams = new List<StatParam>();
        while (randomParams.Count < _upgradesPerLevel)
        {
            var idx = Random.Range(0, GlobalUpgradePath.Upgrades.Count);
            if (randomParams.Contains(GlobalUpgradePath.Upgrades[idx].UpgradeParam))
                continue;
            randomParams.Add(GlobalUpgradePath.Upgrades[idx].UpgradeParam);
        }

        List<GlobalUpgrade> randomUpgrades = new List<GlobalUpgrade>();
        foreach (var param in randomParams)
        {
            randomUpgrades.Add(GlobalUpgradePath.Upgrades.Find(x => x.UpgradeParam == param));
        }

        _uiManager.LevelUp(_currentLevel, _xpThreshold, randomUpgrades);
    }
    public void AddGlobalMod(StatModifier mod)
    {
        _globalMods.Add(mod);
        _uiManager.EndWindow();
        foreach (var wpn in Weapons)
        {
            wpn.ApplyModifiers(new List<StatModifier>() { mod });
        }
        Weapons[ACTIVE].RestartAttack();
        PlayerStats.GetStat(mod.Param)?.AddModifier(mod);
    }
    public void PlayerDeath()
    {
        Game.PlayerDeath();
    }

    #region Weapons
    private void SetWeaponsSource()
    {
        foreach (var weapon in Weapons)
        {
            weapon.SetSource(_attackSource.transform);
        }
    }
    private void SwapRotate()
    {
        _heat.StartCooldown();
        Weapons[ACTIVE].StopAttack();
        Weapons[ACTIVE].ClearSourcedModifiers(Weapons[PASSIVE]);
        _uiManager.SwapAllAnim(Weapons);

        var first = Weapons[0];
        for (int i = 0; i < Weapons.Length - 1; i++)
        {
            Weapons[i] = Weapons[i + 1];
        }
        Weapons[Weapons.Length - 1] = first;
        Weapons[ACTIVE].ApplyModifiers(Weapons[PASSIVE].WeaponData.PassiveModifiers);
        Weapons[ACTIVE].StartAttack();
    }
    private void SwapSlots(int idxA, int idxB)
    {
        if (idxA == 0 || idxB == 0)
        {
            _heat.StartCooldown();
            Weapons[ACTIVE].StopAttack();
        }
        Weapons[ACTIVE].ClearSourcedModifiers(Weapons[PASSIVE]);

        var tmp = Weapons[idxA];
        Weapons[idxA] = Weapons[idxB];
        Weapons[idxB] = tmp;

        _uiManager.Swap2Anim(idxA, idxB, Weapons);
        Weapons[ACTIVE].ApplyModifiers(Weapons[PASSIVE].WeaponData.PassiveModifiers);
        if (idxA == 0 || idxB == 0)
        {
            Weapons[ACTIVE].StartAttack();
        }
    }
    private void ValidateModsAndAssignSource()
    {
        foreach (var upgrade in GlobalUpgradePath.Upgrades)
        {
            foreach (var mod in upgrade.Modifiers)
            {
                mod.Source = this;
                mod.Param = upgrade.UpgradeParam;
                mod.ValidateOrder();
            }
        }
    }

    public List<string> GetWeaponNames()
    {
        return new List<string>
        {
            Weapons[ACTIVE].WeaponData.WeaponName,
            Weapons[PASSIVE].WeaponData.WeaponName,
            Weapons[ABILITY].WeaponData.WeaponName,
        };
    }
    #endregion
}
