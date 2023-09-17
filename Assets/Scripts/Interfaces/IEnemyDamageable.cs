using UnityEngine;

public interface IEnemyDamageable : IDamageable
{
    public EnemyBaseSO EnemyData { get; set; }
    public GameObject DropOnDeath { get; set; }
}
