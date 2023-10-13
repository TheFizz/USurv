using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceData
{
    private readonly Vector3 _forceSource;
    private readonly float _forceStrength;
    private readonly float _massIncreasePerc;
    public Vector3 ForceSource => _forceSource;
    public float ForceStrength => _forceStrength;
    public float MassIncreasePerc => _massIncreasePerc;

    public ForceData(Vector3 forceSource, float forceStrength, float massIncreasePerc)
    {
        _forceSource = forceSource;
        _forceStrength = forceStrength;
        _massIncreasePerc = massIncreasePerc;
    }

}
