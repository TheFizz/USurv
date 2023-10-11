using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/PrayerEffect")]
public class PrayerEffectSO : EffectSO
{
    public float DamagePerStack;
    public override TimedEffect InitializeEffect(NewEnemyBase enemy)
    {
        return new TimedPrayerEffect(this, enemy);
    }
}
