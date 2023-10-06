using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI DebugWeapon;
    [SerializeField] private TextMeshProUGUI DebugAbility;
    [SerializeField] private TextMeshProUGUI DebugPlayer;
    // Start is called before the first frame update
    void Start()
    {
        Game.PSystems.OnDebugText += OnDebugText;
    }

    private void OnDebugText(string text, string destination)
    {
        switch (destination)
        {
            case "WEAPON":
                WriteWeapon(text);
                break;
            case "PLAYER":
                WritePlayer(text);
                break;
            case "ABILITY":
                WriteAbility(text);
                break;
            default:
                break;
        }
    }

    public void WriteWeapon(string text)
    {
        DebugWeapon.text = text;

    }
    public void WritePlayer(string text)
    {
        DebugPlayer.text = text;

    }
    public void WriteAbility(string text)
    {
        DebugAbility.text = text;
    }
    private void OnDestroy()
    {
        Game.PSystems.OnDebugText -= OnDebugText;
    }
}
