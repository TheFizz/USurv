using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrinketSlotButton : MonoBehaviour
{
    public TextMeshProUGUI SlotText;
    public Button Button;
    private void Awake()
    {
        Button = GetComponent<Button>();
    }
    public void SetText(string text)
    {
        SlotText.text = text;
    }
    public void SetColor(Color color)
    {
        SlotText.color = color;
    }
}
