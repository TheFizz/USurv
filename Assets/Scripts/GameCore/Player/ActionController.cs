using RobinGoodfellow.CircleGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private GameObject _attackSource;

    private InputHandler _input;
    private HeatSystem _heat;
    private StatModifierTracker _statModifierTracker;
    private UIManager _uiManager;

    public GameObject[] WeaponQueue = new GameObject[3];
    private WeaponBase[] _weapons = new WeaponBase[3];

    // Start is called before the first frame update
    private void Awake()
    {
        _input = Globals.Input;
        _heat = Globals.Heat;
        _statModifierTracker = Globals.StatModTracker;
        _uiManager = Globals.UIManager;

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
        if (_input.SwapWeapon && _heat.CanSwap())
            SwapRotate();

        if (_input.Swap01 && _heat.CanSwap())
            SwapSlots(0, 1);

        if (_input.Swap12 && _heat.CanSwap())
            SwapSlots(1, 2);

        if (_input.UseAbility)
            _weapons[0].UseAbility();

    }
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
            _weapons[0].ApplyModifiers(_weapons[1].WeaponModifiers);
            _weapons[0].StartAttack();
        }
    }
    private void SwapRotate()
    {
        _heat.StartCooldown();
        _weapons[0].StopAttack();
        _weapons[0].ClearModifiers();
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
        _weapons[0].ClearModifiers();

        var tmp = _weapons[idxA];
        _weapons[idxA] = _weapons[idxB];
        _weapons[idxB] = tmp;

        _uiManager.Swap2Anim(idxA, idxB, _weapons);

        ActivateTopWeapon((idxA == 0 || idxB == 0));
    }
}
