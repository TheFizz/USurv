using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IFoo
{
    public float VarA { get; set; }
    public float VarB { get; set; }
}
public interface IBar : IFoo
{
    public float VarC { get; set; }
    public Baz BA { get; set; }
}
public class Baz
{
    public float A;
    public float B;
}
