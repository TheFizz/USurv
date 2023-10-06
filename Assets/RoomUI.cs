using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{

    public TextMeshProUGUI RoomGoal;
    public TextMeshProUGUI RoomKills;

    // Start is called before the first frame update
    void Awake()
    {
        Game.Instance.OnLevelReady += OnLevelReady;
    }

    private void OnLevelReady(Game game)
    {
        Game.Instance.OnLevelReady -= OnLevelReady;

        Game.Room.OnGoalChanged += OnGoalChanged;
        Game.Room.OnKillsChanged += OnKillsChanged;
        OnKillsChanged(Game.Room.GetCurrentKills());
        OnGoalChanged(Game.Room.GetCurrentGoal());
    }

    private void OnKillsChanged(float value)
    {
        RoomKills.text = value.ToString();
    }

    private void OnGoalChanged(float value)
    {
        RoomGoal.text = value.ToString();
    }
    private void OnDestroy()
    {
        Game.Room.OnGoalChanged -= OnGoalChanged;
        Game.Room.OnKillsChanged -= OnKillsChanged;
    }
}
