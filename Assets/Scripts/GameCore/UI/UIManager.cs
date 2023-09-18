using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class UIManager : MonoBehaviour
{
    private GameObject _levelupWindowInstance;
    public GameObject LevelUpWindow;

    public GameObject HpBar;
    public GameObject XpBar;
    public GameObject HeatBar;

    public TextMeshProUGUI DebugText;
    public TextMeshProUGUI HpText;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI SwapMode;

    public TextMeshProUGUI CurXpText;
    public TextMeshProUGUI MaxXpText;

    public Canvas MainCanvas;

    private HeatSystem _heat;
    private PlayerSystems _pSystems;

    private Image _heatFill;
    private Slider _hpSlider;
    private Slider _heatSlider;
    private Slider _xpSlider;

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
        _pSystems = Globals.PlayerSystems;
        HpText.text = Mathf.RoundToInt(_pSystems.Stats.GetStat(StatParam.PlayerMaxHealth).Value).ToString();
        LevelText.text = "1";

        _heatSlider = HeatBar.GetComponent<Slider>();
        _heatFill = HeatBar.GetComponentInChildren<Image>();

        _hpSlider = HpBar.GetComponent<Slider>();
        _hpSlider.maxValue = _pSystems.Stats.GetStat(StatParam.PlayerMaxHealth).Value;

        _xpSlider = XpBar.GetComponent<Slider>();
        _xpSlider.maxValue = _pSystems.Stats.XPThresholdBase;
        MaxXpText.text = _pSystems.Stats.XPThresholdBase.ToString("0.00");

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
        _hpSlider.value = _pSystems.DmgHandler.CurrentHealth;

        HpText.text = Mathf.RoundToInt(_pSystems.DmgHandler.CurrentHealth).ToString();
        SwapMode.text = Globals.Input.swapMode.ToString();

        _xpSlider.value = _pSystems.CurrentXP;
        CurXpText.text = _pSystems.CurrentXP.ToString("0.00");
    }

    public void SetupWeaponIcons(WeaponBase[] weaponQueue)
    {
        for (int i = 0; i < weaponQueue.Length; i++)
        {
            weaponQueue[i].UIImage.color = SlotColors[i];
            weaponQueue[i].UIImage.transform.SetParent(MainCanvas.transform, false);
            weaponQueue[i].UIObject.transform.SetAsFirstSibling();
            weaponQueue[i].UIRect.anchoredPosition = SlotPositions[i];
            weaponQueue[i].UIRect.sizeDelta = SlotSizes[i];
        }
    }

    public void LevelUp(int level, float threshold, List<StatModifier> perks)
    {
        Time.timeScale = 0;
        Globals.Input.SetInputEnabled(false);

        LevelText.text = level.ToString();
        _xpSlider.maxValue = threshold;
        MaxXpText.text = threshold.ToString("0.00");

        _levelupWindowInstance = Instantiate(LevelUpWindow, MainCanvas.transform);
        var rect = _levelupWindowInstance.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, Screen.height / 4);
        var window = _levelupWindowInstance.GetComponent<LevelupModal>();
        window.Show(perks);
    }
    public void EndLevelup()
    {
        Time.timeScale = 1;
        Globals.Input.SetInputEnabled(true);

        Destroy(_levelupWindowInstance);
    }
    public void SwapAllAnim(WeaponBase[] weaponQueue)
    {
        float animTime = 0.3f;
        for (int i = 0; i < weaponQueue.Length; i++)
        {
            var k = i - 1;
            if (k < 0)
                k = weaponQueue.Length - 1;

            weaponQueue[i].UIRect.DOAnchorPos(SlotPositions[k], animTime, true);
            weaponQueue[i].UIRect.DOSizeDelta(SlotSizes[k], animTime, true);
            weaponQueue[i].UIImage.DOColor(SlotColors[k], animTime);
            weaponQueue[i].UIObject.transform.SetAsFirstSibling();
        }
    }
    public void Swap2Anim(int idxA, int idxB, WeaponBase[] weaponQueue)
    {
        float animTime = 0.3f;

        var A = weaponQueue[idxA].UIObject;
        var B = weaponQueue[idxB].UIObject;

        weaponQueue[idxA].UIRect.DOAnchorPos(SlotPositions[idxA], animTime, true);
        weaponQueue[idxA].UIRect.DOSizeDelta(SlotSizes[idxA], animTime, true);
        weaponQueue[idxA].UIImage.DOColor(SlotColors[idxA], animTime);

        weaponQueue[idxB].UIRect.DOAnchorPos(SlotPositions[idxB], animTime, true);
        weaponQueue[idxB].UIRect.DOSizeDelta(SlotSizes[idxB], animTime, true);
        weaponQueue[idxB].UIImage.DOColor(SlotColors[idxB], animTime);

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
    public void WriteDebug(string text)
    {
        DebugText.text = text;
    }
}
