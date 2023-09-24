using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{
    public GameObject GameUI;
    private List<Vector2> SlotPositions = new List<Vector2>()
    {
        new Vector2(-185,100),//150,150 Size
        new Vector2(-135,200), //100,100 Size
        new Vector2(-85,100) //100,100 Size
    };
    private List<Vector2> SlotSizes = new List<Vector2>()
    {
        new Vector2(150,150),//150,150 Size
        new Vector2(100,100), //100,100 Size
        new Vector2(100,100) //100,100 Size
    };
    private List<Color> SlotColors = new List<Color>()
    {
        Color.white,
        new Color32(128,128,128,255),
        new Color32(134,255,145,255),
    };

    private float _animTime = 0.3f;

    void Awake()
    {
        Globals.PSystems.OnWeaponIconAction += OnWeaponIconAction;
    }

    private void OnWeaponIconAction(List<WeaponBase> weapons, bool swap = false, int idxA = -1, int idxB = -1)
    {
        if (idxA < 0 && idxB < 0)
        {
            if (swap)
                SwapAll(weapons);
            else
                SetIcons(weapons);
        }
        if (idxA >= 0 && idxB >= 0)
        {
            SwapTwo(idxA, idxB, weapons);
        }
    }

    public void SwapAll(List<WeaponBase> weaponQueue)
    {
        for (int i = 0; i < weaponQueue.Count; i++)
        {
            var k = i - 1;
            if (k < 0)
                k = weaponQueue.Count - 1;

            weaponQueue[i].UIRect.DOAnchorPos(SlotPositions[k], _animTime, true);
            weaponQueue[i].UIRect.DOSizeDelta(SlotSizes[k], _animTime, true);
            weaponQueue[i].UIImage.DOColor(SlotColors[k], _animTime);
            weaponQueue[i].UIObject.transform.SetAsFirstSibling();
        }
    }
    public void SwapTwo(int idxA, int idxB, List<WeaponBase> weaponQueue)
    {
        var A = weaponQueue[idxA].UIObject;
        var B = weaponQueue[idxB].UIObject;

        weaponQueue[idxA].UIRect.DOAnchorPos(SlotPositions[idxA], _animTime, true);
        weaponQueue[idxA].UIRect.DOSizeDelta(SlotSizes[idxA], _animTime, true);
        weaponQueue[idxA].UIImage.DOColor(SlotColors[idxA], _animTime);

        weaponQueue[idxB].UIRect.DOAnchorPos(SlotPositions[idxB], _animTime, true);
        weaponQueue[idxB].UIRect.DOSizeDelta(SlotSizes[idxB], _animTime, true);
        weaponQueue[idxB].UIImage.DOColor(SlotColors[idxB], _animTime);

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
            weaponQueue[i].UIImage.color = SlotColors[i];
            weaponQueue[i].UIImage.transform.SetParent(GameUI.transform, false);
            weaponQueue[i].UIObject.transform.SetAsFirstSibling();
            weaponQueue[i].UIRect.anchoredPosition = SlotPositions[i];
            weaponQueue[i].UIRect.sizeDelta = SlotSizes[i];
        }
    }
}
