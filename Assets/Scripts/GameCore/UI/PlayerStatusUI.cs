using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    public GameObject HpBar;
    public GameObject XpBar;

    private TextMeshProUGUI _hpText;
    private TextMeshProUGUI _levelText;
    private TextMeshProUGUI _curXpText;
    private TextMeshProUGUI _maxXpText;

    private Slider _hpSlider;
    private Slider _xpSlider;

    private void Awake()
    {
        Globals.PDamageManager.OnHpChanged += OnHpChanged;
        Globals.PSystems.OnXpChanged += OnXpChanged;
        Globals.PSystems.OnLevelUp += OnLevelUp;

        _hpSlider = HpBar.GetComponent<Slider>();
        _xpSlider = XpBar.GetComponent<Slider>();
        _hpText = HpBar.GetComponentInChildren<TextMeshProUGUI>();
        List<TextMeshProUGUI> xpComp = new List<TextMeshProUGUI>(XpBar.GetComponentsInChildren<TextMeshProUGUI>());
        _levelText = xpComp.Find(c => c.name == "LevelText");
        _curXpText = xpComp.Find(c => c.name == "CurXpText");
        _maxXpText = xpComp.Find(c => c.name == "MaxXpText");
    }
    private void OnXpChanged(float newCurrentXp)
    {
        SetXPValue(newCurrentXp);
    }

    private void OnLevelUp(float newCurrentXp, float newMaxXP, int level, List<GlobalUpgrade> upgrades = null)
    {
        SetXPMax(newMaxXP);
        SetXPValue(newCurrentXp);

        if (_levelText != null)
            _levelText.text = level.ToString();
    }

    private void OnHpChanged(float newCurrentHp, float newMaxHp = -1)
    {
        if (newMaxHp > 0)
            SetHPMax(newMaxHp);
        SetHPValue(newCurrentHp);
    }
    public void SetHPMax(float value)
    {
        _hpSlider.maxValue = value;
    }
    void SetXPMax(float value)
    {
        _xpSlider.maxValue = value;
        if (_maxXpText != null)
            _maxXpText.text = value.ToString();
    }
    public void SetHPValue(float value)
    {
        _hpSlider.value = value;
        if (_hpText != null)
            _hpText.text = value.ToString();
    }
    public void SetXPValue(float value)
    {
        _xpSlider.value = value;
        if (_curXpText != null)
            _curXpText.text = value.ToString();
    }
}
