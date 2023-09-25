using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsUI : MonoBehaviour
{
    public GameObject GameUI;
    public GameObject LevelUpWindow;
    public GameObject UpgradeWindow;

    private void Start()
    {
        Globals.PSystems.OnLevelUp += OnLevelUp;
        Globals.PSystems.OnWeaponPickup += OnWeaponPickup;
    }

    private void OnLevelUp(float newCurrentXp, float newMaxXP, int level, List<GlobalUpgrade> upgrades)
    {
        if (upgrades == null)
            return;

        var window = CreateWindow(LevelUpWindow);
        window.OnWindowClose += OnWindowClose;
        window.ShowUpgrades(upgrades);
    }

    public void OnWeaponPickup(List<WeaponBase> currentWeapons, InteractionType type, string pickupName)
    {
        var window = CreateWindow(UpgradeWindow);
        window.OnWindowClose += OnWindowClose;
        window.ShowWeapons(currentWeapons, type, pickupName);
    }
    ModalWindow CreateWindow(GameObject prefab)
    {
        Time.timeScale = 0;
        Globals.InputHandler.SetInputEnabled(false);
        RoomManager.Instance.PlayerInMenu = true;

        var window = Instantiate(prefab, GameUI.transform);

        var rect = window.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 80);
        return window.GetComponent<ModalWindow>();
    }
    private void OnWindowClose(ModalWindow source)
    {
        Time.timeScale = 1;
        Globals.InputHandler.SetInputEnabled(true);
        RoomManager.Instance.PlayerInMenu = false;

        source.OnWindowClose -= OnWindowClose;
        Destroy(source.transform.gameObject);
    }
}
