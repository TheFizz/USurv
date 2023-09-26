using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIcon : MonoBehaviour
{
    [SerializeField] private Image _heatOL;
    [SerializeField] private Image _abilityOl;
    [SerializeField] private Image _weaponSprite;

    private Color _overheatColor = Color.red;
    private Color _cooledColor = Color.white;

    WeaponBase _weapon;
    public void SetPartner(WeaponBase weapon)
    {
        _weapon = weapon;
        _weapon.OnAbilityFillChanged += OnAbilityFillChanged;
        _weapon.OnHeatFillChanged += OnHeatFillChanged;
        _weapon.OnWeaponOverheated += OnWeaponOverheated;
        _weaponSprite.sprite = _weapon.WeaponData.UIWeaponSprite;
    }

    private void OnHeatFillChanged(float fill)
    {
        _heatOL.fillAmount = fill;
    }

    private void OnAbilityFillChanged(float fill)
    {
        _abilityOl.fillAmount = 1 - fill;
    }

    private void OnWeaponOverheated(bool state)
    {
        if (state)
            _weaponSprite.color = _overheatColor;
        else
            _weaponSprite.color = _cooledColor;
    }
}
