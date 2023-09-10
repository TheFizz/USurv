using RobinGoodfellow.CircleGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private GameObject _activeWeaponGo;
    private IWeapon _activeWeapon;
    public GameObject _attackSource;
    private InputHandler _input;
    [SerializeField] Queue<GameObject> _weaponQueue = new Queue<GameObject>(2);

    // Start is called before the first frame update
    private void Awake()
    {
        var ps = GameObject.FindGameObjectWithTag("PeristentSystems");
        if (ps != null)
            _input = ps.GetComponent<InputHandler>();
        //_weaponQueue.Enqueue((GameObject)Resources.Load("Prefabs/Weapons/WeaponBow"));
        //_weaponQueue.Enqueue((GameObject)Resources.Load("Prefabs/Weapons/WeaponKama"));

        _weaponQueue.Enqueue((GameObject)Instantiate((GameObject)Resources.Load("Prefabs/Weapons/WeaponBow"), transform.position, transform.rotation, transform));
        _weaponQueue.Enqueue((GameObject)Instantiate((GameObject)Resources.Load("Prefabs/Weapons/WeaponKama"), transform.position, transform.rotation, transform));
        foreach (var wpn in _weaponQueue)
        {
            wpn.SetActive(false);
        }
    }
    void Start()
    {
        ActivateTopWeapon();
    }

    void ActivateTopWeapon()
    {
        _activeWeaponGo = _weaponQueue.Peek();
        _activeWeaponGo.SetActive(true);
        _activeWeapon = _activeWeaponGo.GetComponent<IWeapon>();
        _activeWeapon.StartAttack(_attackSource.transform);
    }

    private void Update()
    {
        if (_input.SwapWeapon)
            SwapWeapon();
    }

    private void SwapWeapon()
    {
        _activeWeapon.StopAttack();
        _weaponQueue.Enqueue(_weaponQueue.Dequeue());
        ActivateTopWeapon();
    }

}
