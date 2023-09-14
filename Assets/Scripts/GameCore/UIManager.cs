using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    HeatSystem heat;
    PlayerStats stats;

    public GameObject weaponIcons;

    public TextMeshProUGUI _hpText;

    public GameObject heatBar;
    public GameObject hpBar;

    private Slider _heatSlider;
    private Image _heatFill;

    private Slider _hpSlider;

    private Color _heatColorDefault = new Color32(252, 140, 3, 255);
    private Color _heatColorCooling = new Color32(47, 128, 204, 255);
    private Color _heatColorOverheated = new Color32(219, 31, 31, 255);
    private void Awake()
    {
        heat = Globals.Heat;
        stats = Globals.PlayerTransform.GetComponent<PlayerStats>();

        _heatSlider = heatBar.GetComponent<Slider>();
        _heatFill = heatBar.GetComponentInChildren<Image>();
        _hpSlider = hpBar.GetComponent<Slider>();
        _hpText.text = Mathf.RoundToInt(stats.CurrentHealth).ToString();
        _hpSlider.maxValue = stats.MaxHealth;
    }
    private void Update()
    {
        switch (heat.GetHeatStatus())
        {
            case HeatStatus.Default:
                _heatFill.color = _heatColorDefault;
                break;
            case HeatStatus.Cooling:
                _heatFill.color = _heatColorCooling;
                break;
            case HeatStatus.Overheated:
                _heatFill.color = _heatColorOverheated;
                break;
            default:
                break;
        }

        _heatSlider.value = heat.GetHeat();
        _hpSlider.value = stats.CurrentHealth;
        _hpText.text = Mathf.RoundToInt(stats.CurrentHealth).ToString();
    }
    public void AnimateSwapAll(GameObject[] WeaponQueue)
    {
        foreach (GameObject wpnGo in WeaponQueue)
        {
            wpnGo.GetComponent<WeaponBase>().UIIcon.GetComponent<Animator>().SetTrigger("Rotate");
        }
        WeaponQueue[0].GetComponent<WeaponBase>().UIIcon.transform.SetAsLastSibling();
    }
    public void AnimateSwap2(GameObject tOut, GameObject tIn)
    {
        tOut.GetComponent<WeaponBase>().UIIcon.GetComponent<Animator>().SetTrigger("RevRotate");
        tIn.GetComponent<WeaponBase>().UIIcon.GetComponent<Animator>().SetTrigger("Rotate");
        tOut.GetComponent<WeaponBase>().UIIcon.SetAsFirstSibling();
    }
}
