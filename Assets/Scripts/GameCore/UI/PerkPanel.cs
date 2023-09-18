using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkPanel : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] TextMeshProUGUI _perkText;
    [SerializeField] TextMeshProUGUI _buttonText;
    [SerializeField] Image _image;
    StatModifier _perk;

    public void Setup(StatModifier perk)
    {
        _perk = perk;
        _perkText.text = _perk.ToStringWithBreak();
        _buttonText.text = "Get!";
        _image.sprite = _perk.GetSprite();
        _button.onClick.AddListener(ApplyPerk);
    }
    private void ApplyPerk()
    {
        Globals.PlayerSystems.AddGlobalMod(_perk);
    }
}
