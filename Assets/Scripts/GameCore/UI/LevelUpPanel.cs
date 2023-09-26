using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPanel : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] TextMeshProUGUI _upgradeText;
    [SerializeField] TextMeshProUGUI _buttonText;
    [SerializeField] Image _image;
    GlobalUpgrade _upgrade;
    StatModifier _mod;
    ModalWindow _parent;

    public void Setup(GlobalUpgrade upgrade, ModalWindow parent)
    {
        _parent = parent;
        _upgrade = upgrade;
        _mod = _upgrade.Modifiers[_upgrade.UpgradeNumber];
        _upgradeText.text = _mod.ToStringWithBreaks();
        _buttonText.text = "Get!";
        _levelText.text = $"Level {_upgrade.UpgradeNumber + 1}";
        _image.sprite = Globals.ParamReference[_mod.Param].Image;
        _button.onClick.AddListener(ApplyUpgrade);
    }
    private void ApplyUpgrade()
    {
        _upgrade.UpgradeNumber++;
        Globals.PSystems.AddGlobalMod(_mod);
        _parent.CloseWindow();
    }
}
