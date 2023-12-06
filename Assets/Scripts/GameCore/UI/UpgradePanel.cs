using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] TextMeshProUGUI _slfText;
    [SerializeField] TextMeshProUGUI _pasText;
    [SerializeField] TextMeshProUGUI _ablText;
    [SerializeField] TextMeshProUGUI _buttonText;
    [SerializeField] Image _image;
    WeaponUpgrade _upgrade;
    WeaponBase _weapon;
    ModalWindow _parent;

    public void Setup(WeaponBase weapon, string buttonText, ModalWindow parent)
    {
        _parent = parent;
        _weapon = weapon;
        _upgrade = _weapon.WeaponData.UpgradePath.Find(x => x.UpgradeNumber == _weapon.WeaponLevel + 1);
        if (_upgrade == null)
        {
            Destroy(gameObject);
            return;
        }
        _slfText.text = "(SLF) " + _upgrade.SelfStatMods[0].ToString();
        _pasText.text = "(PAS) " + _upgrade.PassiveStatMods[0].ToString();
        _ablText.text = "(ABL) " + _upgrade.AbilityStatMods[0].ToString();
        if (buttonText.Contains("Replace"))
        {
            Destroy(_slfText.transform.gameObject);
            Destroy(_pasText.transform.gameObject);
            Destroy(_ablText.transform.gameObject);
        }
        _buttonText.text = buttonText;
        _levelText.text = $"Level {weapon.WeaponLevel + 1}";
        _image.sprite = weapon.WeaponData.UIWeaponSprite;
        _button.onClick.AddListener(ApplyUpgrade);
    }
    private void ApplyUpgrade()
    {
        Game.PSystems.AddWeaponUpgrade(_weapon, _weapon.WeaponLevel + 1);
        _parent.CloseWindow();
    }
}
