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
        _weaponSprite.sprite = _weapon.WeaponData.UIWeaponSprite;
    }

    private void OnAbilityFillChanged(float fill)
    {
        _abilityOl.fillAmount = 1 - fill;
    }

    private void OnDestroy()
    {
        _weapon.OnAbilityFillChanged -= OnAbilityFillChanged;
    }
}
