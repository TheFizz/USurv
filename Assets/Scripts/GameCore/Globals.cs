using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Globals : MonoBehaviour
{
    public const string PARAMICONLOC = "UI/Sprites/Parameters";

    private static Globals me;
    public static InputHandler InputHandler { get; set; }
    public static PlayerSystems PSystems { get; set; }
    public static PlayerDamageManager PDamageManager { get; set; }
    public static PlayerInteractionManager PInteractionManager { get; set; }
    public static Dictionary<string, XPDrop> XPDropsPool { get; set; } = new Dictionary<string, XPDrop>();
    public static Dictionary<string, NewEnemyBase> EnemyPool { get; set; } = new Dictionary<string, NewEnemyBase>();
    public static Transform PlayerTransform { get; set; }
    public static Camera MainCamera { get; set; }
    public static Spawner Spawner { get; set; }

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

        MainCamera = Camera.main;
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
