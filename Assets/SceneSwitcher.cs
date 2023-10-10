using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public static SceneSwitcher Instance;
    public event Action<string> OnSceneLoaded;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void LoadScene(string name, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (name == "BootScene")
            Game.Instance?.Destroy();

        SceneManager.LoadScene(name, mode);
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 0;
        OnSceneLoaded?.Invoke(scene.name);
        if (scene.name == "BootScene")
            LoadScene("Room0");
    }

    internal void RestartGame()
    {
        LoadScene("BootScene");
    }
}
