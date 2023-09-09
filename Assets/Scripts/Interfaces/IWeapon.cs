using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IWeapon
{
    public abstract void Attack();
    public abstract void StartAttack(Transform source);
    public abstract void StopAttack(Transform source);
}