using RobinGoodfellow.CircleGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyLayer;
    private IWeapon _weapon;
    public GameObject _attackSource;
    // Start is called before the first frame update
    void Start()
    {
        GameObject w1 = (GameObject)Resources.Load("Prefabs/Weapons/WeaponKama");
        w1 = Instantiate(w1, transform.position, transform.rotation, transform);
        _weapon = w1.GetComponent<IWeapon>();
        _weapon.StartAttack(_attackSource.transform);
    }
}
