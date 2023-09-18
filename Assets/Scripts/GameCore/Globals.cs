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
    public static StatModifierTracker StatModTracker { get; private set; }
    public static PlayerSystems PlayerSystems { get; private set; }
    public static PlayerDamageHandler DmgHandler { get; private set; }
    public static Dictionary<string, XPDrop> XPDropsPool { get; private set; } = new Dictionary<string, XPDrop>();
    public static UIManager UIManager { get; private set; }
    public static Transform PlayerTransform { get; private set; }
    public static Camera MainCamera { get; private set; }

    public static List<StatModifier> AvailablePerks = new List<StatModifier>();

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
        StatModTracker = GetComponent<StatModifierTracker>();

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
        foreach (StatModType modType in StatModType.GetValues(typeof(StatModType)))
        {
            if (modType == StatModType.Flat) continue;
            foreach (StatParam param in StatParam.GetValues(typeof(StatParam)))
            {
                for (int i = 5; i < 30; i += 5)
                {
                    AvailablePerks.Add(new StatModifier(i, modType, param));
                }
            }
        }
    }
    public static bool IsInLayerMask(int layer, LayerMask layerMask) { return layerMask == (layerMask | (1 << layer)); }
}
