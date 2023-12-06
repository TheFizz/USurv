using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrinketPanel : MonoBehaviour
{
    [SerializeField] GameObject TrinketSlotButton;
    [SerializeField] TextMeshProUGUI PanelHeader;
    [SerializeField] Image _image;
    WeaponBase _weapon;
    private TrinketSO _newTrinket;
    ModalWindow _parent;
    public GameObject TrinketPickup;

    public void Setup(WeaponBase weapon, ModalWindow parent, TrinketSO newTrinket)
    {
        PanelHeader.text = $"{weapon.WeaponData.WeaponName} trinkets";
        _weapon = weapon;
        _newTrinket = newTrinket;
        _parent = parent;
        _image.sprite = weapon.WeaponData.UIWeaponSprite;

        for (int i = 0; i < weapon.PassiveTrinkets.Count; i++)
        {
            var trinket = weapon.PassiveTrinkets[i];
            var btn = Instantiate(TrinketSlotButton, gameObject.transform);
            var tsb = btn.GetComponent<TrinketSlotButton>();
            var ttrig = btn.AddComponent<TooltipTrigger>();
            if (trinket == null)
            {
                tsb.SetText("Empty Slot");
                tsb.SetColor(Color.grey);
                ttrig.Header = "Empty slot";
                ttrig.Content = "Put your new trinket here";
                var cap = i;
                tsb.Button.onClick.AddListener(() => AddTrinket(_weapon, newTrinket, cap));

            }
            else
            {
                tsb.SetText(trinket.Name);
                ttrig.Header = trinket.Name;
                ttrig.Content = trinket.Description;
                var cap = i;
                tsb.Button.onClick.AddListener(() => AddTrinket(_weapon, newTrinket, cap));
            }
        }
    }

    private void AddTrinket(WeaponBase wpn, TrinketSO trinket, int SlotIndex)
    {
        if (wpn.PassiveTrinkets[SlotIndex] == null)
            wpn.PassiveTrinkets[SlotIndex] = trinket;
        else
        {
            var pickup = Instantiate(TrinketPickup, Game.PSystems.PlayerObject.transform.position, Quaternion.identity);
            pickup.GetComponent<TrinketInteraction>().SetTrinket(wpn.PassiveTrinkets[SlotIndex]);
            wpn.PassiveTrinkets[SlotIndex] = trinket;
        }
        _parent.CloseWindow();
    }
}
