using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    public HeatSystem heat;
    public PlayerStats stats;

    public Image activeImage;
    public Image passiveImage;
    public Image abilityImage;

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
        _heatSlider = heatBar.GetComponent<Slider>();
        _heatFill = heatBar.GetComponentInChildren<Image>();

        _hpSlider = hpBar.GetComponent<Slider>();
        _hpSlider.maxValue = stats.MaxHealth;
        _hpText.text = Mathf.RoundToInt(stats.CurrentHealth).ToString();
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
    public void SetWeaponImages(GameObject[] weapons)
    {

        activeImage.sprite = weapons[0].GetComponent<WeaponBase>().GetWeaponImage();
        passiveImage.sprite = weapons[1].GetComponent<WeaponBase>().GetWeaponImage();
        abilityImage.sprite = weapons[2].GetComponent<WeaponBase>().GetWeaponImage();

        //heatSlider.image.color;
    }
}
