using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrioStatPanel : MonoBehaviour
{
    public TextMeshProUGUI Header;
    public Transform Content;
    public GameObject TextPrefab;
    public void SetHeader(string header)
    {
        Header.text = header;
    }
    public void AddText(string text)
    {
        Instantiate(TextPrefab, Content).GetComponent<TextMeshProUGUI>().text = text;
    }
}
