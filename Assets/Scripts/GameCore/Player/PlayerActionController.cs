using RobinGoodfellow.CircleGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionController : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private GameObject _attackSource;

    private WeaponBase _passiveWeapon;
    private WeaponBase _abilityWeapon;
    private WeaponBase _activeWeapon;

    private InputHandler _input;
    private HeatSystem _heat;
    private StatModifierTracker _statModifierTracker;
    private UIManager _uiManager;

    public GameObject[] WeaponQueue = new GameObject[3];

    // Start is called before the first frame update
    private void Awake()
    {
        _input = Globals.Input;
        _heat = Globals.Heat;
        _statModifierTracker = Globals.StatModTracker;
        _uiManager = Globals.UIManager;

        for (int i = 0; i < WeaponQueue.Length; i++)
        {
            WeaponQueue[i] = Instantiate(WeaponQueue[i]);
        }

        ActualizePositions();
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
            _abilityWeapon.UseAbility();

    }
    private void SetWeaponsSource()
    {
        _activeWeapon.SetSource(_attackSource.transform);
        _passiveWeapon.SetSource(_attackSource.transform);
        _abilityWeapon.SetSource(_attackSource.transform);
    }
    void ActivateTopWeapon(bool modAndStart = true)
    {
        ActualizePositions();
        if (modAndStart)
        {
            _activeWeapon.ApplyModifiers(_passiveWeapon.WeaponModifiers);
            _activeWeapon.StartAttack();
        }
    }
    void ActualizePositions()
    {
        _activeWeapon = WeaponQueue[0].GetComponent<WeaponBase>();
        _passiveWeapon = WeaponQueue[1].GetComponent<WeaponBase>();
        _abilityWeapon = WeaponQueue[2].GetComponent<WeaponBase>();
    }
    private void SwapRotate()
    {
        _heat.StartCooldown();
        _activeWeapon.StopAttack();
        _activeWeapon.ClearModifiers();

        var first = WeaponQueue[0];
        for (int i = 0; i < WeaponQueue.Length - 1; i++)
        {
            WeaponQueue[i] = WeaponQueue[i + 1];
        }
        WeaponQueue[WeaponQueue.Length - 1] = first;

        //_uiManager.AnimateSwapAll(WeaponQueue);
        ActualizePositions();
        ActivateTopWeapon();
    }
    private void SwapSlots(int idxOld, int idxNew)
    {
        _heat.StartCooldown();
        if (idxOld == 0 || idxNew == 0)
        {
            _activeWeapon.StopAttack();
        }
        _activeWeapon.ClearModifiers();

        var tmp = WeaponQueue[idxOld];
        WeaponQueue[idxOld] = WeaponQueue[idxNew];
        WeaponQueue[idxNew] = tmp;


        //_uiManager.AnimateSwap2(WeaponQueue[idxNew], WeaponQueue[idxOld]);
        ActualizePositions();
        ActivateTopWeapon((idxOld == 0 || idxNew == 0));
    }
}
