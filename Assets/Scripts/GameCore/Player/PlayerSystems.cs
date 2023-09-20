
using RobinGoodfellow.CircleGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerSystems : MonoBehaviour
{
    public PlayerStatsSO Stats;
    private int _perksPerLevel = 3;

    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private GameObject _attackSource;
    [SerializeField] private List<StatModifier> _globalMods = new List<StatModifier>();

    private InputHandler _input;
    private HeatSystem _heat;
    private UIManager _uiManager;
    public PlayerDamageHandler DmgHandler;
    private PlayerMovementController _movementController;

    private float _currentHealth;
    private float _moveSpeed;

    public GameObject[] WeaponQueue = new GameObject[3];
    private WeaponBase[] _weapons = new WeaponBase[3];

    private float _xpThreshold;
    public float CurrentXP = 0;
    private float _xpThresholdMultipltier;

    public int _currentLevel = 1;

    // Start is called before the first frame update
    private void Awake()
    {
        _currentHealth = Stats.GetStat(StatParam.PlayerMaxHealth).Value;
        _moveSpeed = Stats.GetStat(StatParam.PlayerMoveSpeed).Value;
        _xpThreshold = Stats.XPThresholdBase;
        _xpThresholdMultipltier = Stats.XPThresholdMultiplier;

        _heat = Globals.Heat;
        _input = Globals.Input;
        _uiManager = Globals.UIManager;

        _movementController = Globals.PlayerTransform.GetComponent<PlayerMovementController>();
        _movementController.SetMoveSpeed(_moveSpeed);

        DmgHandler = Globals.PlayerTransform.GetComponent<PlayerDamageHandler>();
        DmgHandler.SetMaxHealth(_currentHealth);

        for (int i = 0; i < WeaponQueue.Length; i++)
        {
            var go = WeaponQueue[i] = Instantiate(WeaponQueue[i]);
            _weapons[i] = go.GetComponent<WeaponBase>();
        }
        _uiManager.SetupWeaponIcons(_weapons);

        SetWeaponsSource();
        ActivateTopWeapon();
    }
    private void Update()
    {
        Debug.DrawRay(_attackSource.transform.position, _attackSource.transform.forward * 10, Color.green);

        if (_input.SwapWeapon && _heat.CanSwap())
            SwapRotate();

        if (_input.Swap01 && _heat.CanSwap())
            SwapSlots(0, 1);

        if (_input.Swap12 && _heat.CanSwap())
            SwapSlots(1, 2);

        if (_input.UseAbility)
            _weapons[2].UseAbility();
        if (Input.GetKeyDown("k"))
            _weapons[2].UpgradeToLevel(1);
        if (Input.GetKeyDown("l"))
            _weapons[2].UpgradeToLevel(2);
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

        foreach (var wpn in _weapons)
        {
            wpn.ApplyModifiers(new List<StatModifier>() { mod });
        }
        _weapons[0].RestartAttack();
    }
    public void PlayerDeath()
    {

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
            _weapons[0].ApplyModifiers(_weapons[1].WeaponData.PassiveModifiers);
            _weapons[0].StartAttack();
        }
    }
    private void SwapRotate()
    {
        _heat.StartCooldown();
        _weapons[0].StopAttack();
        _weapons[0].ClearLocalModifiers();
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
            _weapons[0].StopAttack();
        }
        _weapons[0].ClearLocalModifiers();

        var tmp = _weapons[idxA];
        _weapons[idxA] = _weapons[idxB];
        _weapons[idxB] = tmp;

        _uiManager.Swap2Anim(idxA, idxB, _weapons);

        ActivateTopWeapon((idxA == 0 || idxB == 0));
    }
    #endregion
}
