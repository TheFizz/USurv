using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyMoveable 
{
    public Rigidbody _RB { get; set; }
    public float MoveSpeed { get; set; }
    void MoveEnemy(Vector3 target);
}
