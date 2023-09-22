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

    private bool _isDead = false;


    private List<string> _recentAttackers = new List<string>();
    void Awake()
    {
        _pSystems = Globals.PlayerSystems;
    }
    void Start()
    {
        MaxHealth = _pSystems.PlayerStats.GetStat(StatParam.PlayerMaxHealth).Value;
        CurrentHealth = MaxHealth;
    }
    void Update()
    {
        var pHealth = MaxHealth;
        MaxHealth = _pSystems.PlayerStats.GetStat(StatParam.PlayerMaxHealth).Value;
        if (MaxHealth > pHealth)
            CurrentHealth += MaxHealth - pHealth;
    }

    public void Damage(float damageAmount, string attackerID, bool overrideITime = false)
    {
        if (_isDead)
            return;

        if (!overrideITime)
        {
            if (_recentAttackers.Contains(attackerID))
                return;
            if (_invulnerable)
                return;
            _recentAttackers.Add(attackerID);
            if (!_preInv)
            {
                StartCoroutine(PreInv(_preInvDelay));
            }
        }
        CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0 && !_isDead)
        {
            _isDead = true;
            CurrentHealth = 0;
            Die();
        }
    }

    public void Die()
    {
        _invulnerable = true;
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
