using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentTrinketPanel : MonoBehaviour
{
    public TextMeshProUGUI Header;
    public TextMeshProUGUI Content;

    public void SetText(string header, string content)
    {
        Header.text = header;
        Content.text = content;
    }
}
