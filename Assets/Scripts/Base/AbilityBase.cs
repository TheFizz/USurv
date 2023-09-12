using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityState
{
    Ready,
    Cooldown
}
public abstract class AbilityBase : ScriptableObject
{
    public string AbilityName;
    public float AbilityCooldown;
    public abstract void Use(Transform source);
}
