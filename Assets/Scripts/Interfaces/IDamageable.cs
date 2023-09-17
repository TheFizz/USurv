using System.Collections;
using System.Collections.Generic;

public interface IDamageable
{
    // Start is called before the first frame update
    abstract void Damage(float damageAmount, bool overrideITime = false);
    public abstract void Die();
    public abstract float CurrentHealth { get; set; }
    public abstract float MaxHealth { get; set; }
}
