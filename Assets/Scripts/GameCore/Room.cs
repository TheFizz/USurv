using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{

    public event Action<float> OnGoalChanged;
    public event Action<float> OnKillsChanged;
    public event Action<GameObject> OnRoomCreated;

    public TextMeshProUGUI TollText, GoalText;
    public bool PlayerInMenu = false;

    private int _killGoal;
    private int _killCount = 0;
    public RoomState State = RoomState.Active;

    private EndScreen _endScreen;
    public bool UnlimitedPlay;
    public GameObject Reward;
    public GameObject RewardSpot;
    public GameObject Portal;
    public bool RewardTaken = false;
    public GameObject PlayerPrefab;
    public GameObject UIPrefab;
    public GameObject GameUI;
    public GameObject DeathUI;
    public GameObject DeathScreenPrefab;

    internal float GetCurrentKills()
    {
        return _killCount;
    }

    internal float GetCurrentGoal()
    {
        return _killGoal;
    }

    void Awake()
    {
        _endScreen = GetComponent<EndScreen>();
    }
    private void Start()
    {
        Time.timeScale = 1;
        Game.InputHandler.SetInputEnabled(true);

        _killGoal = Random.Range(50, 100);
        //_killGoal = 1;
        OnGoalChanged?.Invoke(_killGoal);
        OnKillsChanged?.Invoke(_killCount);

        OnRoomCreated?.Invoke(null);
    }
    public void Update()
    {
        if (UnlimitedPlay)
        {
            GoalText.text = "∞";
            return;
        }

        if (State == RoomState.Active && _killCount >= _killGoal)
        {
            _killCount = _killGoal;
            NextLevel();
        }
        if (State == RoomState.EndingXP && Game.XPDropsPool.Count < 1)
        {
            SpawnReward();
            State = RoomState.RewardSpawned;
        }
        if (State == RoomState.RewardSpawned && RewardTaken)
        {
            SpawnPortal();
            State = RoomState.PortalSpawned;
        }
    }

    private void SpawnPortal()
    {
        var i = Instantiate(Portal);
        i.transform.position = new Vector3(0, 0, 0);
    }

    private void SpawnReward()
    {
        //FindObjectOfType<RewardGenerator>().InstantiateRewards();
        
        var i = Instantiate(Reward);
        i.AddComponent<WeaponInteraction>();
        i.transform.position = new Vector3(0, 0, 5);
        
    }

    public void PlayerDeath()
    {
        var ds = Instantiate(DeathScreenPrefab, new Vector3(0, -100, 0), Quaternion.identity);
        ds.GetComponent<EndScreen>().Play();
    }
    public void ReloadScene()
    {
        Debug.Log("ReloadScene");
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartGame()
    {
        Debug.Log("RestartGame");
        Time.timeScale = 1;
        Destroy(Game.PSystems.gameObject);
        Game.Instance.Destroy();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        Game.Spawner.StopSpawn();
        Game.PSystems.StopAttack();
        StartCoroutine(KillEnemies());
    }
    public void KillIncrease(int amount)
    {
        _killCount += amount;
        OnKillsChanged?.Invoke(_killCount);
    }

    IEnumerator KillEnemies()
    {
        State = RoomState.EndingEnemy;
        yield return new WaitForSeconds(.2f);
        foreach (var enemy in Game.EnemyPool.Values)
        {
            Destroy(enemy.gameObject);
        }
        Game.EnemyPool.Clear();
        StartCoroutine(CollectXP());
    }
    IEnumerator CollectXP()
    {
        yield return new WaitForSeconds(.2f);
        foreach (var xp in Game.XPDropsPool.Values)
        {
            xp.Attract(Game.PSystems.PlayerObject.transform);
        }
        State = RoomState.EndingXP;
    }
}
