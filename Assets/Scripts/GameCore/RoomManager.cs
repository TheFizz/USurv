using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{

    public TextMeshProUGUI TollText, GoalText;

    public static RoomManager Instance;
    public bool PlayerInMenu = false;

    public int WinCondition;
    public int KillCount = 0;
    public LevelState State = LevelState.Active;

    private EndScreen _endScreen;
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
        //WinCondition = 10;
        GoalText.text = WinCondition.ToString();
        _endScreen = GetComponent<EndScreen>();
        Instance = this;
    }
    public void Update()
    {
        TollText.text = KillCount.ToString();

        if (Instance.UnlimitedPlay)
        {
            GoalText.text = "∞";
            return;
        }

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

    public void PlayerDeath()
    {
        _endScreen.Play();
    }
    public void ReloadScene()
    {
        Time.timeScale = 1;
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
