using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseWindow : MonoBehaviour
{
    public event Action<PauseWindow> OnPauseWindowClose;
    public Button ResumeButton;
    public Button ExitButton;

    private void Awake()
    {
        ResumeButton.onClick.AddListener(() => OnPauseWindowClose?.Invoke(this));
        ExitButton.onClick.AddListener(() => SceneSwitcher.Instance.GoToMenu());
    }
}
