
using RobinGoodfellow.CircleGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerSystems : MonoBehaviour
{
    public static bool instantiated = false;

    public event LevelUpHandler OnLevelUp;
    public event XpChangedHandler OnXpChanged;
    public event WeaponPickupHandler OnWeaponPickup;
    public event WeaponIconsHandler OnWeaponIconAction;
    public event DebugTextHandler OnDebugText;
    public event Action<float, float> OnAttack;

    const int ACTIVE = 0;
    const int PASSIVE = 1;
    const int ABILITY = 2;

    [HideInInspector] public GlobalUpgradePathSO GlobalUpgradePath;
    [SerializeField] private GlobalUpgradePathSO _defaultGlobalUpgradePath;

    [HideInInspector] public PlayerStatsSO PlayerData;
    [SerializeField] private PlayerStatsSO _defaultPlayerData;

    [SerializeField] private LayerMask _enemyLayer;

    [SerializeField] private GameObject[] _weaponQueue = new GameObject[3];
    [HideInInspector] private List<WeaponBase> _weapons = new List<WeaponBase>(new WeaponBase[3]);

    private float _currentXP = 0;
    private int _upgradesPerLevel = 3;
    private int _currentLevel = 1;

    [HideInInspector] public float CurHealth = -1;

    private List<StatModifier> _globalMods = new List<StatModifier>();

    // Start is called before the first frame update
    private void Awake()
    {
        if (instantiated)
        {
            Destroy(gameObject);
            return;
        }
        if (!instantiated)
            instantiated = true;

        Globals.PSystems = this;
        InstantiateSOs();
        ValidateModsAndAssignSource();

        for (int i = 0; i < _weaponQueue.Length; i++)
        {
            var go = Instantiate(_weaponQueue[i], gameObject.transform);
            _weapons[i] = go.GetComponent<WeaponBase>();
            _weapons[i].SwappedTo(i);
        }
    }
    public void SetSource(Transform source)
    {
        SetWeaponsSource(source);
        SetAnimSource(source);
    }

    private void SetAnimSource(Transform source)
    {
        Globals.PAnimationController.SetSource(source);
    }

    internal void UnSubscribeInteracted()
    {
        Globals.PInteractionManager.OnInteracted -= OnInteracted;
    }

    public void SubscribeInteracted()
    {
        Globals.PInteractionManager.OnInteracted += OnInteracted;
    }

    private void Start()
    {
        _weapons[ACTIVE].ApplyModifiers(_weapons[PASSIVE].WeaponData.PassiveModifiers);
        _weapons[ACTIVE].StartAttack();
        OnLevelUp?.Invoke(_currentXP, PlayerData.XPThresholdBase, _currentLevel);
    }
    public List<WeaponBase> GetWeapons()
    {
        return _weapons;
    }
    public void ForceInvokeStatus()
    {
        OnLevelUp?.Invoke(_currentXP, PlayerData.XPThresholdBase, _currentLevel);
    }

    internal void AddWeaponUpgrade(WeaponBase weapon, int level)
    {
        weapon.UpgradeToLevel(level);
        Globals.Room.RewardTaken = true;
        if (weapon == _weapons[PASSIVE])
        {
            _weapons[ACTIVE].ClearSourcedModifiers(_weapons[PASSIVE]);
            _weapons[ACTIVE].ApplyModifiers(_weapons[PASSIVE].WeaponData.PassiveModifiers);
            _weapons[ACTIVE].RestartAttack();
        }
    }

    private void OnInteracted(InteractionType type, string auxName)
    {
        OnWeaponPickup?.Invoke(_weapons, type, auxName);
    }

    private void InstantiateSOs()
    {
        PlayerData = Instantiate(_defaultPlayerData);
        GlobalUpgradePath = Instantiate(_defaultGlobalUpgradePath);
    }
    private void Update()
    {
        ShowDebug();

        if (Globals.InputHandler.SwapWeapon)
            SwapRotate();

        if (Globals.InputHandler.Swap01)
            SwapSlots(0, 1);

        if (Globals.InputHandler.Swap12)
            SwapSlots(1, 2);

        if (Globals.InputHandler.UseAbility)
            _weapons[ABILITY].UseAbility();
    }

    private void ShowDebug()
    {
        var cd = _weapons[ABILITY].WeaponAbility.AbilityCooldown;
        var cdr = _weapons[ABILITY].WeaponAbility.GetStat(StatParam.CooldownReductionPerc).Value;

        string text = _weapons[ABILITY].WeaponAbility.AbilityName + "\n";
        text += $"Base CD: {cd}\n";
        text += $"Modded CD: {cd - (cd * (cdr / 100))}\n";
        foreach (var stat in _weapons[ABILITY].WeaponAbility.Stats)
        {
            text += $"{stat.Parameter}: {stat.Value}\n";
            foreach (var mod in stat.StatModifiers)
            {
                text += $"# {mod}\n";
            }
        }
        OnDebugText?.Invoke(text, "ABILITY");

        text = "Player\n";
        foreach (var stat in PlayerData.Stats)
        {
            text += $"{stat.Parameter}: {stat.Value}\n";
            foreach (var mod in stat.StatModifiers)
            {
                text += $"# {mod}\n";
            }
        }
        OnDebugText?.Invoke(text, "PLAYER");


        text = _weapons[ACTIVE].WeaponData.WeaponName + "\n";
        foreach (var stat in _weapons[ACTIVE].WeaponData.Stats)
        {
            text += $"{stat.Parameter}: {stat.Value}\n";
            foreach (var mod in stat.StatModifiers)
            {
                text += $"# {mod}\n";
            }
        }
        OnDebugText?.Invoke(text, "WEAPON");

    }

    internal void OnWeaponAttack(float range, float cone)
    {
        if (_weapons[ACTIVE].WeaponData.WeaponName != "Kama")
            return;
        OnAttack?.Invoke(range, cone);
    }

    public void AddXP(float xpValue)
    {
        if (_currentXP + xpValue < PlayerData.XPThresholdBase)
        {
            _currentXP += xpValue;
            OnXpChanged?.Invoke(_currentXP);
        }
        else
        {
            _currentXP = xpValue - (PlayerData.XPThresholdBase - _currentXP);
            LevelUp();
        }
    }
    private void LevelUp()
    {
        _currentLevel++;
        PlayerData.XPThresholdBase *= PlayerData.XPThresholdMultiplier;
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

        OnLevelUp?.Invoke(_currentXP, PlayerData.XPThresholdBase, _currentLevel, randomUpgrades);
    }
    public void AddGlobalMod(StatModifier mod)
    {
        _globalMods.Add(mod);
        foreach (var wpn in _weapons)
        {
            wpn.ApplyModifiers(new List<StatModifier>() { mod });
        }
        _weapons[ACTIVE].RestartAttack();
        PlayerData.GetStat(mod.Param)?.AddModifier(mod);
    }
    public void PlayerDeath()
    {
        Globals.Room.PlayerDeath();
    }

    private void SetWeaponsSource(Transform source)
    {
        foreach (var weapon in _weapons)
        {
            weapon.SetSource(source);
        }
    }
    private void SwapRotate()
    {
        _weapons[ACTIVE].StopAttack();
        _weapons[ACTIVE].ClearSourcedModifiers(_weapons[PASSIVE]);

        OnWeaponIconAction?.Invoke(_weapons, true);

        var first = _weapons[0];
        for (int i = 0; i < _weapons.Count - 1; i++)
        {
            _weapons[i + 1].SwappedTo(i);
            _weapons[i] = _weapons[i + 1];

        }
        first.SwappedTo(_weapons.Count - 1);
        _weapons[_weapons.Count - 1] = first;
        _weapons[ACTIVE].ApplyModifiers(_weapons[PASSIVE].WeaponData.PassiveModifiers);
        _weapons[ACTIVE].StartAttack();
    }
    private void SwapSlots(int idxA, int idxB)
    {
        if (idxA == 0 || idxB == 0)
        {
            _weapons[ACTIVE].StopAttack();
        }
        _weapons[ACTIVE].ClearSourcedModifiers(_weapons[PASSIVE]);

        var tmp = _weapons[idxA];

        _weapons[idxB].SwappedTo(idxA);
        _weapons[idxA] = _weapons[idxB];

        tmp.SwappedTo(idxB);
        _weapons[idxB] = tmp;


        OnWeaponIconAction?.Invoke(_weapons, true, idxA, idxB);
        _weapons[ACTIVE].ApplyModifiers(_weapons[PASSIVE].WeaponData.PassiveModifiers);
        if (idxA == 0 || idxB == 0)
        {
            _weapons[ACTIVE].StartAttack();
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
            _weapons[ACTIVE].WeaponData.WeaponName,
            _weapons[PASSIVE].WeaponData.WeaponName,
            _weapons[ABILITY].WeaponData.WeaponName,
        };
    }
}
