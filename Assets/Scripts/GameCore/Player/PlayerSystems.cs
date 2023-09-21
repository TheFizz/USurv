
using RobinGoodfellow.CircleGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerSystems : MonoBehaviour
{
    const int ACTIVE = 0;
    const int PASSIVE = 1;
    const int ABILITY = 2;

    [HideInInspector] public PlayerStatsSO PlayerStats;
    [SerializeField] private PlayerStatsSO _defaultPlayerStats;

    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private GameObject _attackSource;

    [SerializeField] private GameObject[] _weaponQueue = new GameObject[3];
    private WeaponBase[] _weapons = new WeaponBase[3];

    [HideInInspector] public float CurrentXP = 0;
    private float _xpThreshold;
    private float _xpThresholdMultipltier;
    private int _perksPerLevel = 3;
    private int _currentLevel = 1;

    //[HideInInspector] public PlayerDamageHandler DmgHandler;
    private List<StatModifier> _globalMods = new List<StatModifier>();
    private InputHandler _input;
    private HeatSystem _heat;
    private UIManager _uiManager;

    [SerializeField] private EndScreen _endScreen;

    // Start is called before the first frame update
    private void Awake()
    {
        InstantiateSOs();

        _heat = Globals.Heat;
        _input = Globals.Input;
        _uiManager = Globals.UIManager;
        //DmgHandler = Globals.DmgHandler;

        _xpThreshold = PlayerStats.XPThresholdBase;
        _xpThresholdMultipltier = PlayerStats.XPThresholdMultiplier;

        for (int i = 0; i < _weaponQueue.Length; i++)
        {
            var go = _weaponQueue[i] = Instantiate(_weaponQueue[i]);
            _weapons[i] = go.GetComponent<WeaponBase>();
        }
        _uiManager.SetupWeaponIcons(_weapons);

        SetWeaponsSource();
        ActivateTopWeapon();
    }
    private void InstantiateSOs()
    {
        PlayerStats = Instantiate(_defaultPlayerStats);
    }
    private void Update()
    {

        if (Input.GetKeyDown("k"))
            _weapons[ABILITY].UpgradeToLevel(_weapons[ABILITY].WeaponLevel + 1);

        Debug.DrawRay(_attackSource.transform.position, _attackSource.transform.forward * 10, Color.green);

        if (_input.SwapWeapon && _heat.CanSwap())
            SwapRotate();

        if (_input.Swap01 && _heat.CanSwap())
            SwapSlots(0, 1);

        if (_input.Swap12 && _heat.CanSwap())
            SwapSlots(1, 2);

        if (_input.UseAbility)
            _weapons[ABILITY].UseAbility();
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
        List<StatModifier> perks = new List<StatModifier>();
        for (int i = 0; i < _perksPerLevel; i++)
        {
            var idx = Random.Range(0, Globals.AvailablePerks.Count - 1);
            var perk = Globals.AvailablePerks[idx];
            perks.Add(perk);
            Globals.AvailablePerks.RemoveAt(idx);
        }
        _uiManager.LevelUp(_currentLevel, _xpThreshold, perks);
    }
    public void AddGlobalMod(StatModifier mod)
    {
        _globalMods.Add(mod);
        _uiManager.EndLevelup();

        if (Globals.PlayerParams.Contains(mod.Param))
        {
            var stat = PlayerStats.GetStat(mod.Param);
            if (stat != null)
                stat.AddModifier(mod);
        }
        if (Globals.WeaponParams.Contains(mod.Param))
        {
            foreach (var wpn in _weapons)
            {
                wpn.ApplyModifiers(new List<StatModifier>() { mod });
            }
            _weapons[ACTIVE].RestartAttack();
        }
    }
    public void PlayerDeath()
    {
        _endScreen.End();
    }

    #region Weapons
    private void SetWeaponsSource()
    {
        foreach (var weapon in _weapons)
        {
            weapon.SetSource(_attackSource.transform);
        }
    }
    void ActivateTopWeapon(bool modAndStart = true)
    {
        if (modAndStart)
        {
            _weapons[ACTIVE].ApplyModifiers(_weapons[PASSIVE].WeaponData.PassiveModifiers);
            _weapons[ACTIVE].StartAttack();
        }
    }
    private void SwapRotate()
    {
        _heat.StartCooldown();
        _weapons[ACTIVE].StopAttack();
        _weapons[ACTIVE].ClearSourcedModifiers(_weapons[PASSIVE]);
        _uiManager.SwapAllAnim(_weapons);

        var first = _weapons[0];
        for (int i = 0; i < _weapons.Length - 1; i++)
        {
            _weapons[i] = _weapons[i + 1];
        }
        _weapons[_weapons.Length - 1] = first;
        ActivateTopWeapon();
    }
    private void SwapSlots(int idxA, int idxB)
    {
        if (idxA == 0 || idxB == 0)
        {
            _heat.StartCooldown();
            _weapons[ACTIVE].StopAttack();
        }
        _weapons[ACTIVE].ClearSourcedModifiers(_weapons[PASSIVE]);

        var tmp = _weapons[idxA];
        _weapons[idxA] = _weapons[idxB];
        _weapons[idxB] = tmp;

        _uiManager.Swap2Anim(idxA, idxB, _weapons);

        ActivateTopWeapon((idxA == 0 || idxB == 0));
    }
    public void UpdateMaxHealthUI()
    {

    }
    #endregion
}
