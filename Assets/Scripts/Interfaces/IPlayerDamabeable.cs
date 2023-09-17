using System.Collections;
using System.Collections.Generic;

public interface IPlayerDamageable : IDamageable
{
    public void Damage(float damageAmount, string attackerID, bool overrideITime = false);
}
