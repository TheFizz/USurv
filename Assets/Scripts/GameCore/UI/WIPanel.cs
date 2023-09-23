using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WIPanel : MonoBehaviour
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

    public void Setup(WeaponBase weapon, string buttonText)
    {
        _weapon = weapon;
        _upgrade = _weapon.WeaponData.UpgradePath.Find(x => x.UpgradeNumber == _weapon.WeaponLevel + 1);
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
        _weapon.UpgradeToLevel(_weapon.WeaponLevel + 1);
        Globals.UIManager.EndWindow();
        Game.Instance.RewardTaken = true;
    }
}
