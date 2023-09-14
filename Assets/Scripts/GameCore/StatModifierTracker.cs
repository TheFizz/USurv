using Kryz.CharacterStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModifierTracker : MonoBehaviour
{
   public List<StatModifier> LocalModifiers { get; set; } = new List<StatModifier>();
    public List<StatModifier> GlobalModifiers { get; set; } = new List<StatModifier>();
}
