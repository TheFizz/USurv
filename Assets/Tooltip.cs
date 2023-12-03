using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI Header;
    public TextMeshProUGUI Content;
    public LayoutElement LayoutElement;

    public int WrapLimit;

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
            Header.gameObject.SetActive(false);
        else
        {

            Header.gameObject.SetActive(true);
            Header.text = header;
        }
        Content.text = content;

        int headerLength = Header.text.Length;
        int contentLength = Content.text.Length;
        LayoutElement.enabled = (headerLength > WrapLimit || contentLength > WrapLimit) ? true : false;
    }
    private void Update()
    {

        Vector2 pos = Input.mousePosition;
        transform.position = pos;
    }
}
