using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelupModal : MonoBehaviour
{
    [SerializeField] Transform _content;
    [SerializeField] GameObject _perkPrefab;
    public void Show(List<StatModifier> perks)
    {
        foreach (var perk in perks)
        {
            var perkPanel = Instantiate(_perkPrefab, _content);
            var panelScript = perkPanel.GetComponent<PerkPanel>();
            panelScript.Setup(perk);
        }
    }
}