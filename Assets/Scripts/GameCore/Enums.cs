using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum StatModType
{
    Flat = 100,
    PercentAdd = 200,
    PercentMult = 300,
}
public enum RoomState
{
    Active,
    EndingEnemy,
    EndingXP,
    RewardSpawned,
    PortalSpawned
}

public enum InteractionType
{
    Take,
    Consume,
    Pick
}
public enum StatParam
{
    AttackSpeed = 0,
    AttackDamage = 1,
    AttackCone = 2,
    AttackRange = 3,
    PlayerMoveSpeed = 4,
    PlayerMaxHealth = 5,
    PierceCount = 6,
    ProjectileSpeed = 7,
    CritChancePerc = 8,
    CritMultiplierPerc = 9,
    AbilityPower = 10,
    AbilityRange = 11,
    PickupRange = 12,
    CooldownReductionPerc=13,
    WeaponKnockbackForce = 14
}

public enum AilmentType
{
    Fear,
    Knockback
}

public enum AbilityState
{
    Ready,
    Active,
    Cooldown
}
public enum HeatStatus
{
    None,
    Cooling,
    Overheated
}

public enum SwapMode
{
    Rotate,
    TwoKey
}
