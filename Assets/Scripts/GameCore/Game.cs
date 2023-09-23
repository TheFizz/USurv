using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static bool PlayerInMenu = false;
    public static Game Instance;

    public int WinCondition;
    public int KillCount = 0;
    public LevelState State = LevelState.Active;

    private static EndScreen _endScreen;
    public bool UnlimitedPlay;
    public GameObject Reward;
    public GameObject Portal;
    public bool RewardTaken = false;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("There is more than one instance of Game!");
            Destroy(gameObject);
            return;
        }
        WinCondition = Random.Range(200, 400);
        _endScreen = GetComponent<EndScreen>();
        Instance = this;
    }
    public void Update()
    {
        if (Instance.UnlimitedPlay)
            return;

        if (State == LevelState.Active && KillCount >= WinCondition)
        {
            KillCount = WinCondition;
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

    public static void PlayerDeath()
    {
        _endScreen.Play();
    }
    public static void ReloadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public static void QuitGame()
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
