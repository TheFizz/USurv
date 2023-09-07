using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // Start is called before the first frame update
    void Damage(float damageAmount);
    void Die();
    public abstract float MaxHealth { get; set; }
    public abstract float CurrentHealth { get; set; }
    public abstract Vector2 HealthRange { get; set; }
}
