using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void LevelUpHandler(float newCurrentXp, float newMaxXP, int level, List<GlobalUpgrade> upgrades = null);
public delegate void HpChangedHandler(float newCurrentHp, float newMaxHp = -1);
public delegate void XpChangedHandler(float newCurrentXp);
public delegate void InteractionHandler(List<Tuple<KeyCode, InteractionType>> options, string name = null);
public delegate void InteractedHandler(InteractionType type, string auxName);
public delegate void WeaponPickupHandler(List<WeaponBase> currentWeapons, InteractionType type, string pickupName);
public delegate void WindowCloseHandler(ModalWindow source);
public delegate void WeaponIconsHandler(List<WeaponBase> weaponQueue, bool swap = false, int idxA = -1, int idxB = -1);
public delegate void DebugTextHandler(string text, string destination);
public delegate void OverlayFillHandler(float fill);
public delegate void MovementAnimHandler(float hor, float ver);
public delegate void AttackActionHandler();
