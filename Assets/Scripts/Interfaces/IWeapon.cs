using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public abstract float AttackSpeed { get; set; }
    public abstract float AttackDamage { get; set; }
    public abstract float AttackArc { get; set; }
    public abstract float AttackRange { get; set; }

}
