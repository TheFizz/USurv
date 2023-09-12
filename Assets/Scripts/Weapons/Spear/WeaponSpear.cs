using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpear : WeaponBaseMelee
{

    [SerializeField] private GameObject _abilityGraphics;
    [SerializeField] private float _abilityRange = 7f;
    [SerializeField] private float _abilityCooldown = 10f;
    [SerializeField] private float _abilityTimer;
    private bool _canUseAbility = true;


    public override List<StatModifier> Modifiers { get; } = new List<StatModifier>()
    {
        new StatModifier(50, StatModType.Percent, StatModParameter.AttackRange)
    };

    public override void UseAbility(Transform source)
    {
        if (!_canUseAbility)
        {
            Debug.Log("Ability on cooldown " + (_abilityCooldown - _abilityTimer));
            return;
        }
        base.UseAbility(source);
        _canUseAbility = false;
        var sourceFloored = _source.position;
        sourceFloored.y = 0;

        Instantiate(_abilityGraphics, _source.position, Quaternion.identity);

        Collider[] hitEnemies = Physics.OverlapSphere(sourceFloored, _abilityRange, _weaponData.enemyLayer);
        foreach (var hitEnemy in hitEnemies)
        {
            var enemy = hitEnemy.GetComponent<EnemyBase>();
            enemy.ReceiveTempFearEffect(3);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!_canUseAbility)
        {
            _abilityTimer += Time.deltaTime;
            if (_abilityTimer >= _abilityCooldown)
            {
                _canUseAbility = true;
                _abilityTimer = 0;
            }
        }
    }
}
