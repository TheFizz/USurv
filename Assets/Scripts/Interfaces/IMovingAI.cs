using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovingAI : IMoving
{
    public Transform MainTarget { get; set; }
    public void MoveTo(Vector3 target);
    public void LookAt(Vector3 target);
}
