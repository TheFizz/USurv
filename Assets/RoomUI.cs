using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{

    public TextMeshProUGUI RoomGoal;
    public TextMeshProUGUI RoomKills;
    public GameObject GameUI;
    public GameObject DeathUI;

    // Start is called before the first frame update
    void Awake()
    {
        Globals.Room.OnGoalChanged += OnGoalChanged;
        Globals.Room.OnKillsChanged += OnKillsChanged;
        Globals.Room.GameUI = GameUI;
        Globals.Room.DeathUI = DeathUI;

        var buttons = new List<Button>(DeathUI.GetComponentsInChildren<Button>());
        buttons.Find(b => b.name == "RetryBTN").onClick.AddListener(() => Globals.Room.RestartGame());
        buttons.Find(b => b.name == "QuitBTN").onClick.AddListener(() => Globals.Room.QuitGame());
    }

    private void OnKillsChanged(float value)
    {
        RoomKills.text = value.ToString();
    }

    private void OnGoalChanged(float value)
    {
        RoomGoal.text = value.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnDestroy()
    {

        Globals.Room.OnGoalChanged -= OnGoalChanged;
        Globals.Room.OnKillsChanged -= OnKillsChanged;
    }
}
