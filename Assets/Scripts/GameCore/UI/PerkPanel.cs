using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkPanel : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] TextMeshProUGUI _upgradeText;
    [SerializeField] TextMeshProUGUI _buttonText;
    [SerializeField] Image _image;
    GlobalUpgrade _upgrade;
    StatModifier _mod;

    public void Setup(GlobalUpgrade upgrade)
    {
        _upgrade = upgrade;
        _mod = _upgrade.Modifiers[_upgrade.UpgradeNumber];
        _upgradeText.text = _mod.ToStringWithBreak();
        _buttonText.text = "Get!";
        _levelText.text = $"Level {_upgrade.UpgradeNumber + 1}";
        _image.sprite = Globals.ParamReference[_mod.Param].Image;
        _button.onClick.AddListener(ApplyUpgrade);
    }
    private void ApplyUpgrade()
    {
        _upgrade.UpgradeNumber++;
        Globals.PlayerSystems.AddGlobalMod(_mod);
    }
}
