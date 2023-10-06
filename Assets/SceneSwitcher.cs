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
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //for (int i = 0; i < SceneManager.sceneCount; i++)
        //{
        //    if (SceneManager.GetSceneAt(i).name != scene.name)
        //        SceneManager.UnloadScene(i);
        //}

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
