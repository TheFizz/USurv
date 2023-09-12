using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBow : WeaponBaseRanged
{

    [SerializeField] private float _abilityCooldown = 10f;
    [SerializeField] private float _abilityTimer;

    private bool _canUseAbility = true;
    public override List<StatModifier> Modifiers { get; } = new List<StatModifier>()
    {
        new StatModifier(50, StatModType.Percent, StatModParameter.AttackSpeed)
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
        var floorLayer = LayerMask.GetMask("Floor");
        Ray ray = Camera.main.ScreenPointToRay(_input.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitinfo, layerMask: floorLayer, maxDistance: 300f))
        {
            var player = GameObject.FindWithTag("Player");
            var hitpoint = hitinfo.point;
            hitpoint.y = player.transform.position.y;
            player.transform.position = hitpoint;
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
