using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Globals : MonoBehaviour
{
    public const string PARAMICONLOC = "UI/Sprites/Parameters";

    private static Globals me;

    [SerializeReference] private Transform _playerTransform;
    [SerializeReference] private Camera _mainCamera;
    [SerializeReference] private UIManager _uiManager;
    [SerializeReference] private Spawner _spawner;
    public static HeatSystem Heat { get; private set; }
    public static InputHandler Input { get; private set; }
    public static PlayerSystems PlayerSystems { get; private set; }
    public static PlayerDamageHandler DmgHandler { get; private set; }
    public static Dictionary<string, XPDrop> XPDropsPool { get; private set; } = new Dictionary<string, XPDrop>();
    public static Dictionary<string, NewEnemyBase> EnemyPool { get; private set; } = new Dictionary<string, NewEnemyBase>();
    public static UIManager UIManager { get; private set; }
    public static Transform PlayerTransform { get; private set; }
    public static Camera MainCamera { get; private set; }
    public static Spawner Spawner { get; private set; }

    public static float BaseCritMultiplierPerc = 200f;

    public static Dictionary<StatParam, ParameterInfo> ParamReference;
    void Awake()
    {
        CreateParamReference();
        if (me != null && me != this)
        {
            Debug.LogError("There is more than one instance of Globals!");
            Destroy(gameObject);
            return;
        }
        PlayerTransform = _playerTransform;
        MainCamera = _mainCamera;
        UIManager = _uiManager;
        Spawner = _spawner;

        Heat = GetComponent<HeatSystem>();
        Input = GetComponent<InputHandler>();
        PlayerSystems = GetComponent<PlayerSystems>();
        DmgHandler = PlayerTransform.GetComponent<PlayerDamageHandler>();
        me = this;
    }

    private void CreateParamReference()
    {
        ParamReference = new Dictionary<StatParam, ParameterInfo>()
        {
            {StatParam.AbilityPower, new ParameterInfo("Ability Power", "AP") },
            {StatParam.AttackSpeed, new ParameterInfo("Attack Speed", "APS") },
            {StatParam.AttackDamage, new ParameterInfo("Attack Damage", "ATK") },
            //{StatParam.AbilityPower, new ParameterInfo("Ability Power", "AP") }, //CDR
            {StatParam.AttackCone, new ParameterInfo("Attack Cone", "Cone") },
            {StatParam.CritChancePerc, new ParameterInfo("Crit Chance", "CritC") },
            {StatParam.PlayerMaxHealth, new ParameterInfo("Max Health", "HP") },
            //{StatParam.AbilityPower, new ParameterInfo("Ability Power", "AP") }, //HPR
            {StatParam.PlayerMoveSpeed, new ParameterInfo("Move Speed", "MSPD") },
            {StatParam.PickupRange, new ParameterInfo("Pickup Range", "PRNG") },
            {StatParam.AttackRange, new ParameterInfo("Attack Range", "RNG") }
        };
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
    public static bool IsInLayerMask(int layer, LayerMask layerMask) { return layerMask == (layerMask | (1 << layer)); }

}
