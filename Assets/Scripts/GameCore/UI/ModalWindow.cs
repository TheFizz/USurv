using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModalWindow : MonoBehaviour
{
    public event WindowCloseHandler OnWindowClose;

    [SerializeField] TextMeshProUGUI _headerText;
    [SerializeField] Transform _content;
    [SerializeField] GameObject _perkPrefab;
    public void ShowUpgrades(List<GlobalUpgrade> upgrades)
    {
        foreach (var upgrade in upgrades)
        {
            var perkPanel = Instantiate(_perkPrefab, _content);
            var panelScript = perkPanel.GetComponent<LevelUpPanel>();
            panelScript.Setup(upgrade, this);
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
            var panelScript = perkPanel.GetComponent<UpgradePanel>();
            panelScript.Setup(wpn, $"{actionString} {wpn.WeaponData.WeaponName}", this);
        }
    }
    public void ShowTrinkets(List<WeaponBase> weapons, TrinketSO newTrinket)
    {
        _headerText.text = $"Select trinket slot";
        foreach (var wpn in weapons)
        {
            var panel = Instantiate(_perkPrefab, _content);
            var panelScript = panel.GetComponent<TrinketPanel>();
            panelScript.Setup(wpn, this, newTrinket);
        }
    }
    public void CloseWindow()
    {
        OnWindowClose?.Invoke(this);
        TooltipSystem.Hide();
    }
}
