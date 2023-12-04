using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WindowsUI : MonoBehaviour
{
    public GameObject GameUI;
    public GameObject LevelUpWindow;
    public GameObject UpgradeWindow;
    public GameObject TrinketWindow;
    public GameObject TabWindow;

    GameObject tabtrio;

    private void Start()
    {
        Game.PSystems.OnLevelUp += OnLevelUp;
        Game.PSystems.OnWeaponPickup += OnWeaponPickup;
        Game.PSystems.OnTrinketPickup += OnTrinketPickup;

        Game.InputHandler.OnTabPress += OnTabPress;
        Game.InputHandler.OnTabRelease += OnTabRelease;
    }

    private void OnTabRelease(int a)
    {
        Destroy(tabtrio);

        Game.InputHandler.SetMouseInputEnabled(true);
        Game.Room.PlayerInMenu = false;
    }

    private void OnTabPress(int a)
    {
        if (tabtrio != null)
            return;

        tabtrio = Instantiate(TabWindow, Game.GameUI.transform);
        var trioScript = tabtrio.GetComponent<TabTrio>();

        trioScript.Stats.SetHeader("Level " + Game.PSystems.CurrentLevel);

        trioScript.Stats.AddText($"Health: {Game.PSystems.DamageManager.CurrentHealth}/{Game.PSystems.DamageManager.MaxHealth}");
        trioScript.Stats.AddText($"Experience: {Game.PSystems.CurrentXP}/{Game.PSystems.PlayerData.XPThresholdBase}");

        foreach (var stat in Game.PSystems.PlayerData.Stats)
        {
            trioScript.Stats.AddText($"{stat.Parameter}: {stat.Value}");
        }

        trioScript.Weapons.SetWeaponObj(Game.PSystems.PlayerWeapons);

        ShowWeaponTrio(trioScript.Weapons.WeaponObjects[0]);
        Game.InputHandler.SetMouseInputEnabled(false);
        Game.Room.PlayerInMenu = true;

    }

    public void ShowWeaponTrio(WeaponTrioObj weaponObj)
    {
        var weapon = weaponObj.RefWeapon;
        var trioScript = tabtrio.GetComponent<TabTrio>();
        trioScript.Weapons.AbilityHeader.text = $"Ability - {weapon.WeaponAbility.AbilityName}:";
        trioScript.Weapons.ClearText();
        trioScript.Weapons.SetHeader(weapon.WeaponData.WeaponName);
        trioScript.Weapons.AttackInfo.text = weapon.WeaponData.AttackDescription;
        trioScript.Weapons.AbilityInfo.text = weapon.WeaponAbility.Description;

        trioScript.Weapons.AddText(" ", false, false);
        trioScript.Weapons.AddText("Weapon:", false, true);
        foreach (var stat in weapon.WeaponData.Stats)
        {
            var statString = $"{stat.Parameter}: {stat.Value}";
            var isGreen = false;
            if (stat.StatModifiers.Count > 0)
            {
                var flat = stat.StatModifiers.Where(s => s.Type == StatModType.Flat).ToList();
                var percadd = stat.StatModifiers.Where(s => s.Type == StatModType.PercentAdd).ToList();

                if (flat.Count > 0)
                {
                    float total = 0;

                    foreach (var mod in flat)
                    {
                        total += mod.Value;
                    }
                    statString += $" (+{Mathf.RoundToInt(total).ToString()})";
                    isGreen = true;
                }
                if (percadd.Count > 0)
                {
                    float total = 0;

                    foreach (var mod in percadd)
                    {
                        total += mod.Value;
                    }
                    statString += $" (+{Mathf.RoundToInt(total).ToString()}%)";
                    isGreen = true;
                }

            }
            trioScript.Weapons.AddText(statString, isGreen, false);
            weaponObj.Border.SetActive(true);
            foreach (var wpnObj in trioScript.Weapons.WeaponObjects)
            {
                if (wpnObj.Idx != weaponObj.Idx)
                    wpnObj.Border.SetActive(false);
            }
        }

        trioScript.Weapons.AddText(" ", false, false);
        trioScript.Weapons.AddText("Ability:", false, true);
        foreach (var stat in weapon.WeaponAbility.Stats)
        {
            var statString = $"{stat.Parameter}: {stat.Value}";
            var isGreen = false;
            if (stat.StatModifiers.Count > 0)
            {
                var flat = stat.StatModifiers.Where(s => s.Type == StatModType.Flat).ToList();
                var percadd = stat.StatModifiers.Where(s => s.Type == StatModType.PercentAdd).ToList();

                if (flat.Count > 0)
                {
                    float total = 0;

                    foreach (var mod in flat)
                    {
                        total += mod.Value;
                    }
                    statString += $" +{Mathf.RoundToInt(total).ToString()}";
                    isGreen = true;
                }
                if (percadd.Count > 0)
                {
                    float total = 0;

                    foreach (var mod in percadd)
                    {
                        total += mod.Value;
                    }
                    statString += $" (+{Mathf.RoundToInt(total).ToString()}%)";
                    isGreen = true;
                }

            }
            trioScript.Weapons.AddText(statString, isGreen, false);
            weaponObj.Border.SetActive(true);
            foreach (var wpnObj in trioScript.Weapons.WeaponObjects)
            {
                if (wpnObj.Idx != weaponObj.Idx)
                    wpnObj.Border.SetActive(false);
            }
        }

        for (int i = 0; i < weapon.PassiveTrinkets.Count; i++)
        {
            var trinket = weapon.PassiveTrinkets[i];
            if (trinket == null)
            {
                var info = trioScript.Trinkets.TrinketInfos[i];
                info.Header.text = $"Slot {i + 1}: No trinket";
                info.Content.text = "This slot is empty.";
            }
            else
            {
                var info = trioScript.Trinkets.TrinketInfos[i];
                info.Header.text = $"Slot {i + 1}: {trinket.Name}";
                info.Content.text = trinket.Description;
            }
        }

    }
    private void OnTrinketPickup(List<WeaponBase> arg1, TrinketSO newTrinket)
    {
        var window = CreateWindow(TrinketWindow);
        window.OnWindowClose += OnWindowClose;
        window.ShowTrinkets(arg1, newTrinket);
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
        Game.InputHandler.SetInputEnabled(false);
        Game.Room.PlayerInMenu = true;

        var window = Instantiate(prefab, Game.GameUI.transform);

        var rect = window.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 80);
        return window.GetComponent<ModalWindow>();
    }
    private void OnWindowClose(ModalWindow source)
    {
        Time.timeScale = 1;
        Game.InputHandler.SetInputEnabled(true);
        Game.Room.PlayerInMenu = false;

        source.OnWindowClose -= OnWindowClose;
        Destroy(source.transform.gameObject);
    }
    private void OnDestroy()
    {
        Game.PSystems.OnLevelUp -= OnLevelUp;
        Game.PSystems.OnWeaponPickup -= OnWeaponPickup;
        Game.PSystems.OnTrinketPickup -= OnTrinketPickup;

        Game.InputHandler.OnTabPress -= OnTabPress;
        Game.InputHandler.OnTabRelease -= OnTabRelease;
    }
}
