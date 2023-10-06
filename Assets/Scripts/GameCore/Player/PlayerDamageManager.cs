using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageManager : MonoBehaviour, IPlayerDamageable
{

    public event HpChangedHandler OnHpChanged;
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    public Vector2 HealthRange { get; set; }

    private float _invTime = 0.2f;
    private float _preInvDelay = .5f;
    private bool _invulnerable = false;
    private bool _preInv = false;

    private bool _isDead = false;


    private List<string> _recentAttackers = new List<string>();
    void Start()
    {
        MaxHealth = Game.PSystems.PlayerData.GetStat(StatParam.PlayerMaxHealth).Value;
        if (Game.PSystems.CurHealth > 0)
            CurrentHealth = Game.PSystems.CurHealth;
        else
            CurrentHealth = MaxHealth;
        OnHpChanged?.Invoke(CurrentHealth, MaxHealth);
    }
    void Update()
    {
        var pHealth = MaxHealth;
        MaxHealth = Game.PSystems.PlayerData.GetStat(StatParam.PlayerMaxHealth).Value;
        if (MaxHealth > pHealth)
        {
            CurrentHealth += MaxHealth - pHealth;
            Game.PSystems.CurHealth = CurrentHealth;
            OnHpChanged?.Invoke(CurrentHealth, MaxHealth);
        }
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
        Game.PSystems.CurHealth = CurrentHealth;
        if (CurrentHealth <= 0 && !_isDead)
        {
            _isDead = true;
            CurrentHealth = 0;
            Game.PSystems.CurHealth = CurrentHealth;
            Die();
        }
        OnHpChanged?.Invoke(CurrentHealth);
    }
    public void Die()
    {
        _invulnerable = true;
        Game.PSystems.PlayerDeath();
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
