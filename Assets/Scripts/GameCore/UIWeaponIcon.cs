using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeaponIcon : MonoBehaviour
{
    public void GetNewIcon(int idx)
    {
        gameObject.GetComponent<Image>().sprite = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttackController>().WeaponQueue[idx].GetComponent<WeaponBase>().GetWeaponImage();
    }
}
