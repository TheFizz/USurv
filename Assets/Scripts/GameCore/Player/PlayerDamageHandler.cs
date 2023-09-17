using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageHandler : MonoBehaviour, IDamageable
{
    private PlayerSystems _pSystems;
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    public Vector2 HealthRange { get; set; }

    private float _iTime = 1.5f;
    private bool _damageable = true;
    public void SetMaxHealth(float health)
    {
        MaxHealth = health;
        CurrentHealth = health;
    }

    public void Damage(float damageAmount, bool overrideITime = false)
    {
        if (!_damageable && !overrideITime)
            return;

        CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0)
            Die();
        StartCoroutine(Invulnerability(_iTime));
    }

    public void Die()
    {
        _pSystems.PlayerDeath();
    }

    IEnumerator Invulnerability(float time)
    {
        _damageable = false;
        yield return new WaitForSeconds(time);
        _damageable = true;
    }
}
