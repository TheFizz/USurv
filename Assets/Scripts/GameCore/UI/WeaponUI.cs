using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{
    //public GameObject GameUI;
    private List<Vector2> SlotPositions = new List<Vector2>()
    {
        new Vector2(-125,20),//150,150 Size
        new Vector2(0,20), //100,100 Size
        new Vector2(125,20) //100,100 Size
    };
    private List<Vector2> SlotSizes = new List<Vector2>()
    {
        new Vector2(1,1),//150,150 Size
        new Vector2(.5f,.5f), //100,100 Size
        new Vector2(1,1) //100,100 Size
    };
    private float _animTime = 0.3f;

    private void Awake()
    {
        Game.Instance.OnLevelReady += OnLevelReady;
    }

    private void OnLevelReady(Game obj)
    {
        Game.Instance.OnLevelReady -= OnLevelReady;
        Game.PSystems.OnWeaponIconAction += OnWeaponIconAction;
        SetIcons(Game.PSystems.GetWeapons());
    }
    private void OnWeaponIconAction(List<WeaponBase> weapons, bool swap = false, int idxA = -1, int idxB = -1)
    {
        if (idxA < 0 && idxB < 0)
        {
            if (swap)
                SwapAll(weapons);
        }
        if (idxA >= 0 && idxB >= 0)
        {
            SwapTwo(idxA, idxB, weapons);
        }
    }

    public void SwapAll(List<WeaponBase> weapons)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            var k = i - 1;
            if (k < 0)
                k = weapons.Count - 1;

            weapons[i].UIObject.transform.SetAsFirstSibling();
            weapons[i].UIRect.DOAnchorPos(SlotPositions[k], _animTime, true);
            weapons[i].UIRect.DOScale(SlotSizes[k], _animTime);
        }
    }
    public void SwapTwo(int idxA, int idxB, List<WeaponBase> weapons)
    {
        var A = weapons[idxA].UIObject;
        var B = weapons[idxB].UIObject;

        weapons[idxA].UIRect.DOAnchorPos(SlotPositions[idxA], _animTime, true);
        weapons[idxA].UIRect.DOScale(SlotSizes[idxA], _animTime);

        weapons[idxB].UIRect.DOAnchorPos(SlotPositions[idxB], _animTime, true);
        weapons[idxB].UIRect.DOScale(SlotSizes[idxB], _animTime);

        if (idxA > idxB)
        {
            A.transform.SetAsFirstSibling();
            B.transform.SetAsFirstSibling();
        }
        else
        {
            A.transform.SetAsFirstSibling();
            B.transform.SetAsFirstSibling();
        }
    }
    public void SetIcons(List<WeaponBase> weaponQueue)
    {
        for (int i = 0; i < weaponQueue.Count; i++)
        {
            weaponQueue[i].UIObject.transform.SetParent(Game.GameUI.transform, false);
            weaponQueue[i].UIObject.transform.SetAsFirstSibling();
            weaponQueue[i].UIRect.anchoredPosition = SlotPositions[i];
            weaponQueue[i].UIRect.localScale = SlotSizes[i];
        }
    }
    private void OnDestroy()
    {
        Game.Instance.OnLevelReady -= OnLevelReady;
        Game.PSystems.OnWeaponIconAction -= OnWeaponIconAction;
    }
}
