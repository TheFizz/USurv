using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class RoomManager : MonoBehaviour
{

    public event Action<float> OnGoalChanged;
    public event Action<float> OnKillsChanged;
    public event Action<int> OnRoomStart;

    public TextMeshProUGUI TollText, GoalText;
    public bool PlayerInMenu = false;

    private int _killGoal;
    private int _killCount = 0;
    public LevelState State = LevelState.Active;

    private EndScreen _endScreen;
    public bool UnlimitedPlay;
    public GameObject Reward;
    public GameObject Portal;
    public bool RewardTaken = false;
    public GameObject PlayerPrefab;
    public GameObject UIPrefab;
    public GameObject GameUI;
    public GameObject DeathUI;
    public GameObject DeathScreenPrefab;

    void Awake()
    {

        Globals.Room = this;
        Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        Instantiate(UIPrefab, Vector3.zero, Quaternion.identity);
        _endScreen = GetComponent<EndScreen>();
    }
    private void Start()
    {
        Time.timeScale = 1;
        Globals.InputHandler.SetInputEnabled(true);

        OnRoomStart?.Invoke(0);
        _killGoal = Random.Range(200, 400);
        //_killGoal = 10;
        OnGoalChanged?.Invoke(_killGoal);
        OnKillsChanged?.Invoke(_killCount);
    }
    public void Update()
    {
        if (UnlimitedPlay)
        {
            GoalText.text = "∞";
            return;
        }

        if (State == LevelState.Active && _killCount >= _killGoal)
        {
            _killCount = _killGoal;
            NextLevel();
        }
        if (State == LevelState.EndingXP && Globals.XPDropsPool.Count < 1)
        {
            SpawnReward();
            State = LevelState.RewardSpawned;
        }
        if (State == LevelState.RewardSpawned && RewardTaken)
        {
            SpawnPortal();
            State = LevelState.PortalSpawned;
        }
    }

    private void SpawnPortal()
    {
        var i = Instantiate(Portal);
        i.transform.position = new Vector3(0, 0, 0);
    }

    private void SpawnReward()
    {
        var i = Instantiate(Reward);
        i.transform.position = new Vector3(0, 0, 5);
    }

    public void PlayerDeath()
    {
        var ds = Instantiate(DeathScreenPrefab, new Vector3(0, -100, 0), Quaternion.identity);
        ds.GetComponent<EndScreen>().Play();
    }
    public void ReloadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        Destroy(Globals.PSystems.gameObject);
        Globals.Destroy();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitGame()
    {
        // save any game data here
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void NextLevel()
    {
        Globals.Spawner.StopSpawn();
        StartCoroutine(KillEnemies());
    }
    public void KillIncrease(int amount)
    {
        _killCount += amount;
        OnKillsChanged?.Invoke(_killCount);
    }

    IEnumerator KillEnemies()
    {
        State = LevelState.EndingEnemy;
        yield return new WaitForSeconds(.2f);
        foreach (var enemy in Globals.EnemyPool.Values)
        {
            enemy.Kill();
        }
        Globals.EnemyPool.Clear();
        StartCoroutine(CollectXP());
    }
    IEnumerator CollectXP()
    {
        yield return new WaitForSeconds(.2f);
        foreach (var xp in Globals.XPDropsPool.Values)
        {
            xp.Attract(Globals.PlayerTransform);
        }
        State = LevelState.EndingXP;
    }
}
