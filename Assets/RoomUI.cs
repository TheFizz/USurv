using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomUI : MonoBehaviour
{

    public TextMeshProUGUI RoomGoal;
    public TextMeshProUGUI RoomKills;
    public GameObject GameUI;

    // Start is called before the first frame update
    void Awake()
    {
        Globals.Room.OnGoalChanged += OnGoalChanged;
        Globals.Room.OnKillsChanged += OnKillsChanged;
        Globals.Room.GameUI = GameUI;
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
