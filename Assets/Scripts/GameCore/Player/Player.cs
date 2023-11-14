
using RobinGoodfellow.CircleGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    const int ACTIVE = 0;
    const int PASSIVE = 1;
    const int ABILITY = 2;

    private static Player _instance;

    public event Action<GameObject> OnPlayerSpawned;

    public event LevelUpHandler OnLevelUp;
    public event XpChangedHandler OnXpChanged;
    public event WeaponPickupHandler OnWeaponPickup;
    public event WeaponIconsHandler OnWeaponIconAction;
    public event DebugTextHandler OnDebugText;
    public event Action<float, float> OnAttack;

    private GameObject _playerPrefab;

    [HideInInspector] public PlayerStatsSO PlayerData;
    [SerializeField] private PlayerStatsSO _defaultPlayerData;

    [HideInInspector] public GlobalUpgradePathSO GlobalUpgradePath;
    [SerializeField] private GlobalUpgradePathSO _defaultGlobalUpgradePath;

    [SerializeField] private LayerMask _enemyLayer;

    [SerializeField] private List<GameObject> _defaultWeapons = new List<GameObject>();
    [HideInInspector] public List<WeaponBase> PlayerWeapons = new List<WeaponBase>(new WeaponBase[3]);

    public List<TrinketSO> CurrentTrinkets = null;
    public readonly Dictionary<OnHitStackTimedTrinketSO, OnHitStackTimedTrinket> TimedTrinkets = new Dictionary<OnHitStackTimedTrinketSO, OnHitStackTimedTrinket>();

    public PlayerDamageManager DamageManager { get; private set; }
    public PlayerInteractionManager InteractionManager { get; private set; }
    public PlayerAnimationController AnimationController { get; private set; }
    public PlayerMovementController MovementController { get; private set; }
    public GameObject PlayerObject { get; private set; }
    public Transform AttackSource { get; private set; }
    private float _currentXP = 0;
    private int _upgradesPerLevel = 3;
    private int _currentLevel = 1;

    [HideInInspector] public float CurHealth = -1;

    private List<StatModifier> _activeGlobalMods = new List<StatModifier>();

    // Start is called before the first frame update
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    protected void HandleTimedTrinkets()
    {
        foreach (var trinket in TimedTrinkets.Values.ToList())
        {
            trinket.Tick(Time.deltaTime);
            if (trinket.IsFinished)
            {
                TimedTrinkets.Remove(trinket.TrinketData);
            }
        }
    }
    public void AddTimedTrinket(OnHitStackTimedTrinket trinket)
    {
        if (TimedTrinkets.ContainsKey(trinket.TrinketData))
        {
            TimedTrinkets[trinket.TrinketData].Activate();
        }
        else
        {
            TimedTrinkets.Add(trinket.TrinketData, trinket);
            trinket.Activate();
        }
    }
    public void SpawnPlayer()
    {
        var gate = FindObjectOfType<GateAnimations>();
        var spawnPoint = gate.transform.position;
        spawnPoint.z += 3;
        PlayerObject = Instantiate(_playerPrefab, spawnPoint, Quaternion.identity);
        AttackSource = PlayerObject.transform.Find("AttackSource").transform;
        DamageManager = PlayerObject.GetComponent<PlayerDamageManager>();
        InteractionManager = PlayerObject.GetComponent<PlayerInteractionManager>();
        AnimationController = PlayerObject.GetComponent<PlayerAnimationController>();
        MovementController = PlayerObject.GetComponent<PlayerMovementController>();
        SetSource(AttackSource);
        SetPlayerActive(false);
        SetPlayerLocked(true);
        OnPlayerSpawned?.Invoke(PlayerObject);
    }
    public void SetPlayerActive(bool active)
    {
        PlayerObject.SetActive(active);
        if (!active)
            StopAttack();
        if (active)
            StartAttack();
    }

    public void SetPlayerLocked(bool locked)
    {
        MovementController.lockPosition = locked;
        MovementController.lockRotation = locked;
    }

    public void Initialize(GameObject playerPrefab, PlayerStatsSO dPlayerData, GlobalUpgradePathSO dUpgradePath, LayerMask enemyLayer)
    {
        //TODO: PlayerSys factory
        _playerPrefab = playerPrefab;
        _defaultPlayerData = dPlayerData;
        _defaultGlobalUpgradePath = dUpgradePath;
        _enemyLayer = enemyLayer;

        var defaultWeaponNames = new List<string> { "Kama", "Bow", "Spear" };
        foreach (var name in defaultWeaponNames)
        {
            _defaultWeapons.Add(Resources.Load($"Prefabs/Weapons/Weapon{name}") as GameObject);
        }
        InstantiateData();
        SetupUpgradeMods();
        InstantiateWeapons();
    }

    public void InstantiateWeapons()
    {
        for (int i = 0; i < _defaultWeapons.Count; i++)
        {
            var go = Instantiate(_defaultWeapons[i], gameObject.transform);
            PlayerWeapons[i] = go.GetComponent<WeaponBase>();
            PlayerWeapons[i].SwappedTo(i);
        }
    }

    private void InstantiateData()
    {
        PlayerData = Instantiate(_defaultPlayerData);
        GlobalUpgradePath = Instantiate(_defaultGlobalUpgradePath);
    }
    private void SetupUpgradeMods()
    {
        foreach (var upgrade in GlobalUpgradePath.Upgrades)
        {
            foreach (var mod in upgrade.Modifiers)
            {
                mod.Source = this;
                mod.Param = upgrade.UpgradeParam;
                mod.ValidateOrder();
            }
        }
    }
    public void SetSource(Transform source)
    {
        SetWeaponsSource(source);
        SetAnimSource(source);
    }
    private void SetAnimSource(Transform source)
    {
        AnimationController.SetSource(source);
    }

    internal void UnSubscribeInteracted()
    {
        InteractionManager.OnInteracted -= OnInteracted;
    }

    public void SubscribeInteracted()
    {
        InteractionManager.OnInteracted += OnInteracted;
    }

    private void Start()
    {
        OnLevelUp?.Invoke(_currentXP, PlayerData.XPThresholdBase, _currentLevel);
    }
    public void StartAttack()
    {
        PlayerWeapons[ACTIVE].ApplyModifiers(PlayerWeapons[PASSIVE].WeaponData.PassiveModifiers);
        CurrentTrinkets = PlayerWeapons[PASSIVE].PassiveTrinkets;
        PlayerWeapons[ACTIVE].StartAttack();
    }
    public void StopAttack()
    {
        PlayerWeapons[ACTIVE].ClearSourcedModifiers(PlayerWeapons[PASSIVE]);
        CurrentTrinkets = null;
        PlayerWeapons[ACTIVE].StopAttack();
    }
    public List<WeaponBase> GetWeapons()
    {
        return PlayerWeapons;
    }
    public void ForceInvokeStatus()
    {
        OnLevelUp?.Invoke(_currentXP, PlayerData.XPThresholdBase, _currentLevel);
    }

    internal void AddWeaponUpgrade(WeaponBase weapon, int level)
    {
        weapon.UpgradeToLevel(level);
        Game.Room.RewardTaken = true;
        if (weapon == PlayerWeapons[PASSIVE])
        {
            PlayerWeapons[ACTIVE].ClearSourcedModifiers(PlayerWeapons[PASSIVE]);
            PlayerWeapons[ACTIVE].ApplyModifiers(PlayerWeapons[PASSIVE].WeaponData.PassiveModifiers);
        }
    }

    private void OnInteracted(InteractionType type, string auxName)
    {
        OnWeaponPickup?.Invoke(PlayerWeapons, type, auxName);
    }

    private void Update()
    {
        ShowDebug();
        HandleTimedTrinkets();
        if (Game.InputHandler.SwapWeapon)
            SwapRotate();

        if (Game.InputHandler.Swap01)
            SwapSlots(0, 1);

        if (Game.InputHandler.Swap12)
            SwapSlots(1, 2);

        if (Game.InputHandler.UseAbility)
            PlayerWeapons[ABILITY].UseAbility();
    }

    private void ShowDebug()
    {
        var cd = PlayerWeapons[ABILITY].WeaponAbility.AbilityCooldown;
        var cdr = PlayerWeapons[ABILITY].WeaponAbility.GetStat(StatParam.CooldownReductionPerc).Value;

        string text = PlayerWeapons[ABILITY].WeaponAbility.AbilityName + "\n";
        text += $"Base CD: {cd}\n";
        text += $"Modded CD: {cd - (cd * (cdr / 100))}\n";
        foreach (var stat in PlayerWeapons[ABILITY].WeaponAbility.Stats)
        {
            text += $"{stat.Parameter}: {stat.Value}\n";
            foreach (var mod in stat.StatModifiers)
            {
                text += $"# {mod}\n";
            }
        }
        OnDebugText?.Invoke(text, "ABILITY");

        text = "Player\n";
        foreach (var stat in PlayerData.Stats)
        {
            text += $"{stat.Parameter}: {stat.Value}\n";
            foreach (var mod in stat.StatModifiers)
            {
                text += $"# {mod}\n";
            }
        }
        OnDebugText?.Invoke(text, "PLAYER");


        text = PlayerWeapons[ACTIVE].WeaponData.WeaponName + "\n";
        foreach (var stat in PlayerWeapons[ACTIVE].WeaponData.Stats)
        {
            text += $"{stat.Parameter}: {stat.Value}\n";
            foreach (var mod in stat.StatModifiers)
            {
                text += $"# {mod}\n";
            }
        }
        OnDebugText?.Invoke(text, "WEAPON");

    }

    internal void OnWeaponAttack(float range, float cone)
    {
        if (PlayerWeapons[ACTIVE].WeaponData.WeaponName != "Kama")
            return;
        OnAttack?.Invoke(range, cone);
    }

    public void AddXP(float xpValue)
    {
        if (_currentXP + xpValue < PlayerData.XPThresholdBase)
        {
            _currentXP += xpValue;
            OnXpChanged?.Invoke(_currentXP);
        }
        else
        {
            _currentXP = xpValue - (PlayerData.XPThresholdBase - _currentXP);
            LevelUp();
        }
    }
    private void LevelUp()
    {
        _currentLevel++;
        PlayerData.XPThresholdBase *= PlayerData.XPThresholdMultiplier;
        List<StatParam> randomParams = new List<StatParam>();
        while (randomParams.Count < _upgradesPerLevel)
        {
            var idx = Random.Range(0, GlobalUpgradePath.Upgrades.Count);
            if (randomParams.Contains(GlobalUpgradePath.Upgrades[idx].UpgradeParam))
                continue;
            randomParams.Add(GlobalUpgradePath.Upgrades[idx].UpgradeParam);
        }

        List<GlobalUpgrade> randomUpgrades = new List<GlobalUpgrade>();
        foreach (var param in randomParams)
        {
            randomUpgrades.Add(GlobalUpgradePath.Upgrades.Find(x => x.UpgradeParam == param));
        }

        OnLevelUp?.Invoke(_currentXP, PlayerData.XPThresholdBase, _currentLevel, randomUpgrades);
    }
    public void AddGlobalMod(StatModifier mod)
    {
        _activeGlobalMods.Add(mod);
        foreach (var wpn in PlayerWeapons)
        {
            wpn.ApplyModifiers(new List<StatModifier>() { mod });
        }
        PlayerData.GetStat(mod.Param)?.AddModifier(mod);
    }
    public void PlayerDeath()
    {
        Game.Room.PlayerDeath();
    }

    private void SetWeaponsSource(Transform source)
    {
        foreach (var weapon in PlayerWeapons)
        {
            weapon.SetSource(source);
        }
    }
    private void SwapRotate()
    {
        PlayerWeapons[ACTIVE].StopAttack();
        PlayerWeapons[ACTIVE].ClearSourcedModifiers(PlayerWeapons[PASSIVE]);
        CurrentTrinkets = null;

        OnWeaponIconAction?.Invoke(PlayerWeapons, true);

        var first = PlayerWeapons[0];
        for (int i = 0; i < PlayerWeapons.Count - 1; i++)
        {
            PlayerWeapons[i + 1].SwappedTo(i);
            PlayerWeapons[i] = PlayerWeapons[i + 1];

        }
        first.SwappedTo(PlayerWeapons.Count - 1);
        PlayerWeapons[PlayerWeapons.Count - 1] = first;
        PlayerWeapons[ACTIVE].ApplyModifiers(PlayerWeapons[PASSIVE].WeaponData.PassiveModifiers);
        CurrentTrinkets = PlayerWeapons[PASSIVE].PassiveTrinkets;

        PlayerWeapons[ACTIVE].StartAttack();
    }
    private void SwapSlots(int idxA, int idxB)
    {
        if (idxA == 0 || idxB == 0)
        {
            PlayerWeapons[ACTIVE].StopAttack();
        }
        PlayerWeapons[ACTIVE].ClearSourcedModifiers(PlayerWeapons[PASSIVE]);
        CurrentTrinkets = null;

        var tmp = PlayerWeapons[idxA];

        PlayerWeapons[idxB].SwappedTo(idxA);
        PlayerWeapons[idxA] = PlayerWeapons[idxB];

        tmp.SwappedTo(idxB);
        PlayerWeapons[idxB] = tmp;


        OnWeaponIconAction?.Invoke(PlayerWeapons, true, idxA, idxB);
        PlayerWeapons[ACTIVE].ApplyModifiers(PlayerWeapons[PASSIVE].WeaponData.PassiveModifiers);
        CurrentTrinkets = PlayerWeapons[PASSIVE].PassiveTrinkets;
        if (idxA == 0 || idxB == 0)
        {
            PlayerWeapons[ACTIVE].StartAttack();
        }
    }

    public List<string> GetWeaponNames()
    {
        return new List<string>
        {
            PlayerWeapons[ACTIVE].WeaponData.WeaponName,
            PlayerWeapons[PASSIVE].WeaponData.WeaponName,
            PlayerWeapons[ABILITY].WeaponData.WeaponName,
        };
    }
    private void OnDestroy()
    {
    }
}
