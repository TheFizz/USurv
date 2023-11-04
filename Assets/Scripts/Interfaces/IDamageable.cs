using System.Collections;
using System.Collections.Generic;

public interface IDamageable
{
    public void Damage(float damageAmount, bool isCrit, string attackerID, bool overrideITime = false);
    public void Die();
    public float CurrentHealth { get; set; }
    public float MaxHealth { get; set; }
    public bool Damageable { get; set; }
    public float InDmgFactor { get; set; }
}
