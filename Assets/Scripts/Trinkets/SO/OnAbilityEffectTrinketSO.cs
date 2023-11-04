
using UnityEngine;

[CreateAssetMenu(menuName = "Trinkets/OnAbilityEffect")]
class OnAbilityEffectTrinketSO : TrinketSO
{
    public float ChancePerc;
    public EffectSO Effect;

    public bool Roll()
    {
        int roll = Random.Range(0, 100);
        return roll < ChancePerc;
    }

    public void OnHitAction(NewEnemyBase enemy)
    {
        if (Roll())
            enemy.AddEffect(Effect.InitializeEffect(enemy));
    }
}
