using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class TrioWeaponPanel : MonoBehaviour
{
    public TextMeshProUGUI Header;
    public TextMeshProUGUI AttackInfo;
    public TextMeshProUGUI AbilityInfo;
    public Transform StatContent;
    public GameObject TextPrefab;
    public List<WeaponTrioObj> WeaponObjects;
    public void SetHeader(string header)
    {
        Header.text = header;
    }

    public void AddText(string text, bool isGreen, bool isYellow)
    {
        var line = Instantiate(TextPrefab, StatContent).GetComponent<TextMeshProUGUI>();
        line.text = text;

        if (isGreen)
            line.color = UnityEngine.Color.green;
        if (isYellow)
            line.color = UnityEngine.Color.yellow;
    }
    public void ClearText()
    {
        foreach (Transform child in StatContent)
        {
            Destroy(child.gameObject);
        }
    }
    public void SetWeaponObj(List<WeaponBase> weapons)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            WeaponObjects[i].Border.SetActive(false);
            WeaponObjects[i].Icon.sprite = weapons[i].WeaponData.UIWeaponSprite;
            WeaponObjects[i].RefWeapon = weapons[i];
            WeaponObjects[i].Idx = i;
        }
    }
}
