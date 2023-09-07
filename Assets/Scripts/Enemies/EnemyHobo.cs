using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHobo : EnemyBase
{
    [field: SerializeField] public override float MaxHealth { get; set; }
    [field: SerializeField] public override float MoveSpeed { get; set; } = 3f;
    public override Vector2 HealthRange { get; set; } = new Vector2(.5f, 3.5f);

}
