using RobinGoodfellow.CircleGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private GameObject _activeWeaponGo;
    [SerializeField] private GameObject _attackSource;

    private WeaponBase _passiveWeapon;
    private WeaponBase _abilityWeapon;
    private WeaponBase _activeWeapon;

    private InputHandler _input;
    private HeatSystem _heat;
    private StatModifierTracker _statModifierTracker;
    private UIManager _uiManager;

    public GameObject[] _weaponQueue = new GameObject[3];

    // Start is called before the first frame update
    private void Awake()
    {
        var ps = GameObject.FindGameObjectWithTag("PeristentSystems");
        if (ps != null)
        {
            _input = ps.GetComponent<InputHandler>();
            _heat = ps.GetComponent<HeatSystem>();
            _statModifierTracker = ps.GetComponent<StatModifierTracker>();
            _uiManager = ps.GetComponent<UIManager>();
        }

        _weaponQueue[0] = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/Weapons/WeaponBow"), transform.position, transform.rotation, transform);
        _weaponQueue[1] = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/Weapons/WeaponKama"), transform.position, transform.rotation, transform);
        _weaponQueue[2] = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/Weapons/WeaponSpear"), transform.position, transform.rotation, transform);
        ActivateTopWeapon();
        SetWeaponsSource();
    }
    private void Update()
    {
        if (_input.SwapWeapon && _heat.CanSwap())
            SwapWeapon();

        if (_input.UseAbility)
            _abilityWeapon.UseAbility();
    }
    private void SetWeaponsSource()
    {
        _activeWeapon.SetSource(_attackSource.transform);
        _passiveWeapon.SetSource(_attackSource.transform);
        _abilityWeapon.SetSource(_attackSource.transform);
    }
    void ActivateTopWeapon()
    {
        _uiManager.SetWeaponImages(_weaponQueue);

        _activeWeapon = _weaponQueue[0].GetComponent<WeaponBase>();
        _passiveWeapon = _weaponQueue[1].GetComponent<WeaponBase>();
        _abilityWeapon = _weaponQueue[2].GetComponent<WeaponBase>();

        _statModifierTracker.Modifiers = _passiveWeapon.WeaponModifiers;

        _activeWeapon.StartAttack();
    }
    private void SwapWeapon()
    {
        _heat.StartCooldown();

        _activeWeapon.StopAttack();

        var first = _weaponQueue[0];
        for (int i = 0; i < _weaponQueue.Length - 1; i++)
        {
            _weaponQueue[i] = _weaponQueue[i + 1];
        }
        _weaponQueue[_weaponQueue.Length - 1] = first;

        ActivateTopWeapon();
    }
}
