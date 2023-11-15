using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponInteraction : Interaction
{
    [SerializeField] private TextAsset _weaponRewards;
    private void Start()
    {
        var lines = new string[] {"Bow","Spear","Kama" };
        InteractionTitle = lines[Random.Range(0, lines.Length)];
        Options = new List<Tuple<KeyCode, InteractionType>>
        {
            new Tuple<KeyCode, InteractionType>(KeyCode.E, InteractionType.Take),
            new Tuple<KeyCode, InteractionType>(KeyCode.F, InteractionType.Consume)
        };
    }
}

public class PillarInteraction : Interaction
{
    public string Title;
    private void Start()
    {
        InteractionTitle = Title;
        Options = new List<Tuple<KeyCode, InteractionType>>
        {
            new Tuple<KeyCode, InteractionType>(KeyCode.E, InteractionType.Pick),
        };
    }
}


public class Interaction : MonoBehaviour
{
    [HideInInspector] public string InteractionTitle;
    [HideInInspector] public List<Tuple<KeyCode, InteractionType>> Options = new List<Tuple<KeyCode, InteractionType>>();
}
