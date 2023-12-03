using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverReceiver : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        var obj = GetComponentInParent<WeaponTrioObj>();
        Game.GameUI.GetComponent<WindowsUI>().ShowWeaponTrio(obj);
    }
}
