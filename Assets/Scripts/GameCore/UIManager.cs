using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{

    public GameObject HpBar;
    public GameObject HeatBar;
    public TextMeshProUGUI HpText;
    public TextMeshProUGUI SwapMode;
    public Canvas MainCanvas;

    private HeatSystem _heat;
    private PlayerStats _stats;

    private Image _heatFill;
    private Slider _hpSlider;
    private Slider _heatSlider;

    private Color _heatColorDefault = new Color32(252, 140, 3, 255);
    private Color _heatColorCooling = new Color32(47, 128, 204, 255);
    private Color _heatColorOverheated = new Color32(219, 31, 31, 255);

    private List<Vector2> SlotPositions = new List<Vector2>()
    {
        new Vector2(-185,100),//150,150 Size
        new Vector2(-135,200), //100,100 Size
        new Vector2(-85,100) //100,100 Size
    };
    private List<Vector2> SlotSizes = new List<Vector2>()
    {
        new Vector2(150,150),//150,150 Size
        new Vector2(100,100), //100,100 Size
        new Vector2(100,100) //100,100 Size
    };
    private List<Color> SlotColors = new List<Color>()
    {
        Color.white,
        new Color32(128,128,128,255),
        new Color32(134,255,145,255),
    };


    private void Awake()
    {
        _heat = Globals.Heat;
        _stats = Globals.PlayerTransform.GetComponent<PlayerStats>();

        _heatSlider = HeatBar.GetComponent<Slider>();
        _heatFill = HeatBar.GetComponentInChildren<Image>();
        _hpSlider = HpBar.GetComponent<Slider>();
        HpText.text = Mathf.RoundToInt(_stats.CurrentHealth).ToString();
        _hpSlider.maxValue = _stats.MaxHealth;

    }
    private void Update()
    {
        switch (_heat.GetHeatStatus())
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

        _heatSlider.value = _heat.GetHeat();
        _hpSlider.value = _stats.CurrentHealth;
        HpText.text = Mathf.RoundToInt(_stats.CurrentHealth).ToString();
        SwapMode.text = Globals.Input.swapMode.ToString();

    }

    public void SetupWeaponIcons(GameObject[] weaponQueue)
    {
        for (int i = 0; i < weaponQueue.Length; i++)
        {
            var weapon = weaponQueue[i].GetComponent<WeaponBase>();
            weapon.UIIcon = Instantiate(weapon.WeaponData.UIWeaponIcon);
            weapon.UIIcon.GetComponent<Image>().sprite = weapon.WeaponData.UIWeaponSprite;
            weapon.UIIcon.GetComponent<Image>().color = SlotColors[i];
            weapon.UIIcon.name = weapon.name + "Icon";
            weapon.UIIcon.transform.SetParent(MainCanvas.transform, false);
            weapon.UIIcon.transform.SetAsFirstSibling();

            var rect = weapon.UIIcon.GetComponent<RectTransform>();
            rect.anchoredPosition = SlotPositions[i];
            rect.sizeDelta = SlotSizes[i];
        }
    }
    public void SwapAllAnim(GameObject[] weaponQueue)
    {
        float animTime = 0.3f;
        for (int i = 0; i < weaponQueue.Length; i++)
        {
            var icon = weaponQueue[i].GetComponent<WeaponBase>().UIIcon;
            var k = i - 1;
            if (k < 0)
                k = weaponQueue.Length - 1;
            icon.GetComponent<RectTransform>().DOAnchorPos(SlotPositions[k], animTime, true);
            icon.GetComponent<RectTransform>().DOSizeDelta(SlotSizes[k], animTime, true);
            icon.GetComponent<Image>().DOColor(SlotColors[k], animTime);
            icon.transform.SetAsFirstSibling();
        }
    }

    public void Swap2Anim(int idxA, int idxB, GameObject[] weaponQueue)
    {
        float animTime = 0.3f;

        var A = weaponQueue[idxA].GetComponent<WeaponBase>().UIIcon;
        var B = weaponQueue[idxB].GetComponent<WeaponBase>().UIIcon;

        A.GetComponent<RectTransform>().DOAnchorPos(SlotPositions[idxA], animTime, true);
        A.GetComponent<RectTransform>().DOSizeDelta(SlotSizes[idxA], animTime, true);
        A.GetComponent<Image>().DOColor(SlotColors[idxA], animTime);

        B.GetComponent<RectTransform>().DOAnchorPos(SlotPositions[idxB], animTime, true);
        B.GetComponent<RectTransform>().DOSizeDelta(SlotSizes[idxB], animTime, true);
        B.GetComponent<Image>().DOColor(SlotColors[idxB], animTime);

        if (idxA > idxB)
        {
            A.transform.SetAsFirstSibling();
            B.transform.SetAsFirstSibling();
        }
        else
        {
            A.transform.SetAsFirstSibling();
            B.transform.SetAsFirstSibling();
        }
    }
}
