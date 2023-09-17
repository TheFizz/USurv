using UnityEngine;

public interface IEnemyDamageable : IDamageable
{
    public void Damage(float damageAmount);
    public EnemyBaseSO EnemyData { get; set; }
    public GameObject DropOnDeath { get; set; }
}
