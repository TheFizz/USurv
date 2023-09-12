using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    public float MaxHealth { get; set; } = 20;
    public float CurrentHealth { get; set; }
    public Vector2 HealthRange { get; set; }

    private float _iTime = 1.5f;
    private bool _damageable = true;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
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
        Destroy(gameObject);
    }

    IEnumerator Invulnerability(float time)
    {
        _damageable = false;
        yield return new WaitForSeconds(time);
        _damageable = true;
    }
}
