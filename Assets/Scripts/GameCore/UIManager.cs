using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public Image activeImage;
    public Image passiveImage;
    public Image abilityImage;

    public void SetWeaponImages(GameObject[] weapons)
    {

        activeImage.sprite = weapons[0].GetComponent<WeaponBase>().GetWeaponImage();
        passiveImage.sprite = weapons[1].GetComponent<WeaponBase>().GetWeaponImage();
        abilityImage.sprite = weapons[2].GetComponent<WeaponBase>().GetWeaponImage();
    }
}
