using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IWeapon
{
    void Attack();
    void StartAttack(Transform source);
    void StopAttack();
}