using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    private static Globals me;

    [SerializeReference] private Transform _playerTransform;
    [SerializeReference] private Camera _mainCamera;
    [SerializeReference] private UIManager _uiManager;
    [SerializeReference] public List<Sprite> StatModIcons = new List<Sprite>();

    public static List<Sprite> StatModIconsSt = new List<Sprite>();
    public static HeatSystem Heat { get; private set; }
    public static InputHandler Input { get; private set; }
    public static PlayerSystems PlayerSystems { get; private set; }
    public static PlayerDamageHandler DmgHandler { get; private set; }
    public static Dictionary<string, XPDrop> XPDropsPool { get; private set; } = new Dictionary<string, XPDrop>();
    public static UIManager UIManager { get; private set; }
    public static Transform PlayerTransform { get; private set; }
    public static Camera MainCamera { get; private set; }
    public static Swoosher Swoosher { get; private set; }

    public static float BaseCritMultiplierPerc = 200f;
    public static List<StatModifier> AvailablePerks = new List<StatModifier>();

    public static List<StatParam> PlayerParams = new List<StatParam>()
    {
        StatParam.CritChancePerc,
        StatParam.CritMultiplierPerc,
        StatParam.PickupRange,
        StatParam.PlayerMaxHealth,
        StatParam.PlayerMoveSpeed,
        StatParam.AttackCone,
        StatParam.AttackSpeed
    };
    public static List<StatParam> WeaponParams = new List<StatParam>()
    {
        StatParam.CritChancePerc,
        StatParam.CritMultiplierPerc,
        StatParam.AttackCone,
        StatParam.AttackSpeed
    };

    void Awake()
    {
        if (me != null)
        {
            Debug.LogError("There is more than one instance of Globals!");
            return;
        }
        PlayerTransform = _playerTransform;
        MainCamera = _mainCamera;
        UIManager = _uiManager;
        StatModIconsSt = StatModIcons;

        Heat = GetComponent<HeatSystem>();
        Input = GetComponent<InputHandler>();
        PlayerSystems = GetComponent<PlayerSystems>();
        DmgHandler = PlayerTransform.GetComponent<PlayerDamageHandler>();
        Swoosher = GetComponent<Swoosher>();

        GeneratePerks();

        me = this;
    }

    public static float GetLargestValue(Vector3 v3, bool omitY = false)
    {
        if (omitY)
            v3.y = 0;
        return Mathf.Max(Mathf.Max(v3.x, v3.y), v3.z);
    }

    public static string GenerateId()
    {
        int a = Random.Range(int.MinValue, int.MaxValue);
        int b = Random.Range(int.MinValue, int.MaxValue);
        return $"{a.ToString("x").ToUpperInvariant()}{b.ToString("x").ToUpperInvariant()}";
    }
    public static bool CompareLayers(int layer, LayerMask layerMask)
    {
        return (((1 << layer) & layerMask) == 1);
    }
    private void GeneratePerks()
    {
        foreach (StatParam param in PlayerParams)
        {
            foreach (StatModType modType in StatModType.GetValues(typeof(StatModType)))
            {
                if (param == StatParam.CritMultiplierPerc || param == StatParam.CritChancePerc)
                    if (modType != StatModType.Flat) continue;


                if (modType == StatModType.Flat)
                    for (int i = 1; i < 5; i += 1)
                    {
                        AvailablePerks.Add(new StatModifier(i, modType, param, "GLOBAL"));
                    }
                else
                    for (int i = 5; i < 30; i += 5)
                    {
                        AvailablePerks.Add(new StatModifier(i, modType, param, "GLOBAL"));
                    }
            }
        }

    }
    public static bool IsInLayerMask(int layer, LayerMask layerMask) { return layerMask == (layerMask | (1 << layer)); }
}
