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
            _statModifierTracker = ps.GetComponent<StatModifierTracker>();
            _uiManager = ps.GetComponent<UIManager>();
        }

        _weaponQueue[0] = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/Weapons/WeaponBow"), transform.position, transform.rotation, transform);
        _weaponQueue[1] = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/Weapons/WeaponKama"), transform.position, transform.rotation, transform);
        _weaponQueue[2] = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/Weapons/WeaponSpear"), transform.position, transform.rotation, transform);

        //foreach (var wpn in _weaponQueue)
        //{
        //    wpn.SetActive(false);
        //}

    }
    void Start()
    {
        ActivateTopWeapon();
    }

    void ActivateTopWeapon()
    {
        _uiManager.SetWeaponImages(_weaponQueue);

        _passiveWeapon = _weaponQueue[1].GetComponent<WeaponBase>();
        _abilityWeapon = _weaponQueue[2].GetComponent<WeaponBase>();
        _statModifierTracker.Modifiers = _passiveWeapon.Modifiers;

        _activeWeaponGo = _weaponQueue[0];
       // _activeWeaponGo.SetActive(true);

        _activeWeapon = _activeWeaponGo.GetComponent<WeaponBase>();
        _activeWeapon.StartAttack(_attackSource.transform);
    }

    private void Update()
    {
        if (_input.SwapWeapon)
            SwapWeapon();

        if (_input.UseAbility)
            _abilityWeapon.UseAbility(_attackSource.transform);
    }

    private void SwapWeapon()
    {
        _activeWeapon.StopAttack();
        //_activeWeaponGo.SetActive(false);

        var first = _weaponQueue[0];
        for (int i = 0; i < _weaponQueue.Length - 1; i++)
        {
            _weaponQueue[i] = _weaponQueue[i + 1];
        }
        _weaponQueue[_weaponQueue.Length - 1] = first;

        ActivateTopWeapon();
    }
}
