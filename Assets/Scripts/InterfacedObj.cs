using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfacedObj : MonoBehaviour, IBar
{
    [field: SerializeField] public float VarA { get; set; }
    public float VarB { get; set; }
    public float VarC { get; set; }
    [field: SerializeField] public Baz BA { get; set; }
}
