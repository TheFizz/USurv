using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageHandler : MonoBehaviour, IPlayerDamageable
{
    private PlayerSystems _pSystems;
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    public Vector2 HealthRange { get; set; }

    private float _invTime = 0.2f;
    private float _preInvDelay = .5f;
    private bool _invulnerable = false;
    private bool _preInv = false;

    private List<string> _recentAttackers = new List<string>();
    public void SetMaxHealth(float health)
    {
        MaxHealth = health;
        CurrentHealth = health;
    }

    public void Damage(float damageAmount, string attackerID, bool overrideITime = false)
    {
        if (_recentAttackers.Contains(attackerID))
            return;

        if (_invulnerable && !overrideITime)
            return;

        CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0)
            Die();

        _recentAttackers.Add(attackerID);

        if (!overrideITime)
            if (!_preInv)
            {
                StartCoroutine(PreInv(_preInvDelay));
            }
    }

    public void Die()
    {
        _pSystems.PlayerDeath();
    }

    IEnumerator PreInv(float time)
    {
        _preInv = true;
        yield return new WaitForSeconds(time);
        _preInv = false;
        StartCoroutine(Inv(_invTime));
    }
    IEnumerator Inv(float time)
    {
        _invulnerable = true;
        yield return new WaitForSeconds(time);
        _invulnerable = false;
        _recentAttackers.Clear();
    }
}
