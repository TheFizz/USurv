using UnityEngine;

public interface IEnemyDamageable : IDamageable
{
    public EnemyBaseSO EnemyData { get; set; }
}
