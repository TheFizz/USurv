using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void LevelUpHandler(float newCurrentXp, float newMaxXP, int level, List<GlobalUpgrade> upgrades = null);
public delegate void HpChangedHandler(float newCurrentHp, float newMaxHp = -1);
public delegate void XpChangedHandler(float newCurrentXp);
public delegate void InteractionHandler(List<Tuple<KeyCode, InteractionType>> options, string name = null);
public delegate void InteractedHandler(InteractionType type, Interaction interaction);
public delegate void WeaponPickupHandler(List<WeaponBase> currentWeapons, InteractionType type, string pickupName);
public delegate void WindowCloseHandler(ModalWindow source);
public delegate void WeaponIconsHandler(List<WeaponBase> weaponQueue, bool swap = false, int idxA = -1, int idxB = -1);
public delegate void DebugTextHandler(string text, string destination);
public delegate void OverlayFillHandler(float fill);
public delegate void MovementAnimHandler(Vector3 movedirection, float speed);
public delegate void AttackActionHandler();
public delegate void WeaponOverheatHandler(bool state);