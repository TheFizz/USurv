using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    public GameObject InteractionContainer;
    public GameObject InteractionText;

    private void Start()
    {
        Globals.PInteractionManager.OnInteraction += OnInteraction;
    }
    public void OnInteraction(List<Tuple<KeyCode, InteractionType>> options, string name = null)
    {
        if (options == null)
        {
            foreach (Transform child in InteractionContainer.transform)
            {
                InteractionContainer.transform.parent.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
            return;
        }
        InteractionContainer.transform.parent.gameObject.SetActive(true);
        foreach (var option in options)
        {
            var pf = Instantiate(InteractionText);
            pf.GetComponent<TextMeshProUGUI>().text = $"[{option.Item1}] {option.Item2} {name}";
            pf.transform.SetParent(InteractionContainer.transform, false);
        }
    }
    private void OnDestroy()
    {

        Globals.PInteractionManager.OnInteraction -= OnInteraction;
    }
}
