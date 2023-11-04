using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IForceable
{
    public Vector3 ForceVector { get; set; }
    public float GetMass();
    public Transform GetTransform();
    public void SetMass(float mass);

}
