using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelupModal : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _headerText;
    [SerializeField] Transform _content;
    [SerializeField] GameObject _perkPrefab;
    public void ShowUpgrades(List<GlobalUpgrade> upgrades)
    {
        foreach (var upgrade in upgrades)
        {
            var perkPanel = Instantiate(_perkPrefab, _content);
            var panelScript = perkPanel.GetComponent<PerkPanel>();
            panelScript.Setup(upgrade);
        }
    }
    public void ShowWeapons(List<WeaponBase> weapons, InteractionType type, string pickupName)
    {
        _headerText.text = $"{type} {pickupName}";
        var actionString = "";
        switch (type)
        {
            case InteractionType.Take:
                actionString = "Replace";
                break;
            case InteractionType.Consume:
                actionString = "Upgrade";
                break;
            default:
                break;
        }
        foreach (var wpn in weapons)
        {
            var perkPanel = Instantiate(_perkPrefab, _content);
            var panelScript = perkPanel.GetComponent<WIPanel>();
            panelScript.Setup(wpn, $"{actionString} {wpn.WeaponData.WeaponName}");
        }
    }
}
