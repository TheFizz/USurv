using System.Collections;
using System.Collections.Generic;

public interface IDamageable
{
    public abstract void Die();
    public abstract float CurrentHealth { get; set; }
    public abstract float MaxHealth { get; set; }
}
