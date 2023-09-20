using UnityEngine;

public interface IEnemyDamageable : IDamageable
{
    public void Damage(float damageAmount, bool isCrit);
    public EnemyBaseSO EnemyData { get; set; }
    public GameObject DropOnDeath { get; set; }
}
