using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIcon : MonoBehaviour
{
    [SerializeField] private Image _heatOL;
    [SerializeField] private Image _abilityOl;
    [SerializeField] private Image _weaponSprite;
    WeaponBase _weapon;
    public void SetPartner(WeaponBase weapon)
    {
        _weapon = weapon;
        _weapon.OnAbilityFillChanged += OnAbilityFillChanged;
        _weapon.OnHeatFillChanged += OnHeatFillChanged;
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
}
