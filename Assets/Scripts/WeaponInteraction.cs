using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponInteraction : MonoBehaviour
{
    [SerializeField] private TextAsset _weaponRewards;
    [HideInInspector] public string RewardName;
    [HideInInspector] public List<Tuple<KeyCode, InteractionType>> Options = new List<Tuple<KeyCode, InteractionType>>();
    private void Awake()
    {
        var lines = _weaponRewards.text.Split(Environment.NewLine);
        RewardName = lines[Random.Range(0, lines.Length)];
        Options = new List<Tuple<KeyCode, InteractionType>>
        {
            new Tuple<KeyCode, InteractionType>(KeyCode.E, InteractionType.Take),
            new Tuple<KeyCode, InteractionType>(KeyCode.F, InteractionType.Consume)
        };
    }
    void Update()
    {

    }
}
