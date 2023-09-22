using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static bool PlayerInMenu = false;
    public static Game Instance;

    public static int WinCondition;
    public static int KillCount = 0;
    public static LevelState State = LevelState.Active;

    private static EndScreen _endScreen;
    public bool UnlimitedPlay;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("There is more than one instance of Game!");
            Destroy(gameObject);
            return;
        }
        WinCondition = Random.Range(200, 400);
        //WinCondition = 50;
        _endScreen = GetComponent<EndScreen>();
        //Debug.Log(WinCondition);
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
        if (State == LevelState.Ending && Globals.XPDropsPool.Count < 1)
        {
            State = LevelState.Ended;
            SpawnReward();
        }
    }

    private void SpawnReward()
    {
        Debug.Log("Reward");
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
    {// save any game data here
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void NextLevel()
    {
        Globals.Spawner.StopSpawn();
        State = LevelState.Ending;
        StartCoroutine(KillEnemies());
    }

    IEnumerator KillEnemies()
    {
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
    }
}
