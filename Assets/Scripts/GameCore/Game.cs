using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{

    public const string PARAMICONLOC = "UI/Sprites/Parameters";

    public event Action<Game> OnLevelReady;

    public List<EffectSO> GameEffects = new List<EffectSO>();

    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _uiPrefab;
    [SerializeField] private GameObject _gameUiPrefab;
    [SerializeField] public GameObject DeathUiPrefab;
    [SerializeField] private PlayerStatsSO _defaultPlayerStats;
    [SerializeField] private GlobalUpgradePathSO _defaultUpgradePath;
    [SerializeField] private LayerMask _enemyLayer;

    public static Game Instance;
    public static InputHandler InputHandler { get; set; }
    public static PlayerSystems PSystems { get; set; }
    public static Dictionary<string, XPDrop> XPDropsPool { get; set; } = new Dictionary<string, XPDrop>();
    public static Dictionary<string, NewEnemyBase> EnemyPool { get; set; } = new Dictionary<string, NewEnemyBase>();
    public static Camera MainCamera { get; set; }
    public static Spawner Spawner { get; set; }
    public static Room Room { get; set; }
    public static GameObject UI { get; set; }
    public static GameObject GameUI { get; set; }

    public static float BaseCritMultiplierPerc = 200f;

    public static Dictionary<StatParam, ParameterInfo> ParamReference;
    void Awake()
    {
        CreateParamReference();
        if (Instance != null && Instance != this)
        {
            Debug.Log("There is more than one instance of Globals!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreatePlayerSystems();
        SceneSwitcher.Instance.OnSceneLoaded += OnSceneLoaded;
    }
    private void CreatePlayerSystems()
    {
        PSystems = gameObject.AddComponent<PlayerSystems>();
        PSystems.Initialize(_playerPrefab, _defaultPlayerStats, _defaultUpgradePath, _enemyLayer);
        PSystems.OnPlayerSpawned += OnPlayerSpawned;
    }

    private void OnSceneLoaded(string scene)
    {
        if (scene == "BootScene")
            return;
        BuildUI();
        CreateRoom();
    }

    private void BuildUI()
    {
        UI = Instantiate(_uiPrefab, Vector3.zero, Quaternion.identity);
        GameUI = Instantiate(_gameUiPrefab, UI.transform.GetComponentInChildren<Canvas>().transform);
    }

    private void CreateRoom()
    {
        var room = Resources.Load("Prefabs/Room") as GameObject;
        Room = Instantiate(room).GetComponent<Room>();
        Room.OnRoomCreated += OnRoomCreated;
    }

    private void OnRoomCreated(GameObject camera)
    {
        MainCamera = Camera.main;
        PSystems.SpawnPlayer();
    }
    private void OnPlayerSpawned(GameObject playerObj)
    {
        var cameraPivot = MainCamera.gameObject.transform.parent;
        cameraPivot.GetComponent<CameraFollow>().CenterPoint = PSystems.AttackSource;
        PSystems.StopAttack();
        PSystems.StartAttack();
        OnLevelReady?.Invoke(this);
    }

    private void CreateParamReference()
    {
        //TODO: Remake with SOs
        ParamReference = new Dictionary<StatParam, ParameterInfo>()
        {
            {StatParam.AbilityPower, new ParameterInfo("Ability Power", "AP") },
            {StatParam.AttackSpeed, new ParameterInfo("Attack Speed", "APS") },
            {StatParam.AttackDamage, new ParameterInfo("Attack Damage", "ATK") },
            {StatParam.CooldownReductionPerc, new ParameterInfo("Cooldown Reduction", "CDR") },
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
    public void Destroy()
    {
        //PlayerSystems.instantiated = false;
        Destroy(Instance.gameObject);
    }
    private void OnDestroy()
    {
        SceneSwitcher.Instance.OnSceneLoaded -= OnSceneLoaded;
        Room.OnRoomCreated -= OnRoomCreated;
        PSystems.OnPlayerSpawned -= OnPlayerSpawned;
    }
}
