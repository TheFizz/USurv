using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRanged
{
    public abstract int PierceCount { get; set; }
    public abstract float ProjectileSpeed { get; set; }
    public abstract Object Projectile { get; set; }
}
