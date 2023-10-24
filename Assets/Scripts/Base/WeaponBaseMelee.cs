using DG.Tweening;
using RobinGoodfellow.CircleGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class WeaponBaseMelee : WeaponBase
{
    [SerializeField] private GameObject _damageCone;
    private bool isRight = false;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Attack()
    {
        if (HeatStatus == HeatStatus.Overheated || HeatStatus == HeatStatus.Cooling)
            return;

        base.Attack();

        bool hasHit = false;
        var forward = Source.forward;
        var coneCos = Mathf.Cos((WeaponData.GetStat(StatParam.AttackCone).Value / 2) * Mathf.Deg2Rad);
        var sourceFloored = new Vector3(Source.position.x, 0, Source.position.z);

        Collider[] hitEnemies = Physics.OverlapSphere(sourceFloored, WeaponData.GetStat(StatParam.AttackRange).Value, WeaponData.EnemyLayer);

        List<NewEnemyBase> dmgEnemies = new List<NewEnemyBase>();
        foreach (var hitEnemy in hitEnemies)
        {
            var capsule = ((CapsuleCollider)hitEnemy);
            var maxVectorValue = Game.GetLargestValue(capsule.gameObject.transform.localScale, true);
            var realColRadius = capsule.radius * maxVectorValue;


            Vector3 enemyPos = hitEnemy.transform.position;
            Vector3 enemyPosFloored = new Vector3(enemyPos.x, 0, enemyPos.z);
            Vector3 dirSrcToEnemy = (enemyPosFloored - sourceFloored).normalized;

            var dot = Vector3.Dot(dirSrcToEnemy, forward);
            var d = GetSectorCirclePoints(enemyPosFloored, realColRadius, sourceFloored, forward, WeaponData.GetStat(StatParam.AttackCone).Value, WeaponData.GetStat(StatParam.AttackRange).Value);

            if (dot >= coneCos)
            {
                dmgEnemies.Add(hitEnemy.GetComponent<NewEnemyBase>());
                hasHit = true;
            }

            else if (d.Count > 0)
            {
                dmgEnemies.Add(hitEnemy.GetComponent<NewEnemyBase>());
                hasHit = true;
            }

        }
        if (hasHit)
        {
            AddHeat(1);
            foreach (var trinket in Game.PSystems.CurrentTrinkets)
            {
                if (trinket is OnHitAttackTrinketSO)
                {
                    var OHTrinket = (OnHitAttackTrinketSO)trinket;
                    OHTrinket.OnHitAttack();
                }
                if (trinket is OnHitStackTimedTrinketSO)
                {
                    var OHTrinket = (OnHitStackTimedTrinketSO)trinket;
                    Game.PSystems.AddTimedTrinket(OHTrinket.Init());
                }
            }
        }
        DamageAll(dmgEnemies);
        ShowGraphics();
    }

    private void DamageAll(List<NewEnemyBase> enemies)
    {
        foreach (var enemy in enemies)
        {
            bool isCrit = false;
            int roll = Random.Range(0, 100);
            float chance = WeaponData.GetStat(StatParam.CritChancePerc).Value;
            float dmg = WeaponData.GetStat(StatParam.AttackDamage).Value;
            if (roll < chance)
            {
                dmg *= (WeaponData.GetStat(StatParam.CritMultiplierPerc).Value + Game.BaseCritMultiplierPerc) / 100;
                isCrit = true;
            }
            enemy.Damage(dmg, isCrit);

            var knockback = ((WeaponMeleeSO)WeaponData).KnockbackEffect;
            var force = WeaponData.GetStat(StatParam.WeaponKnockbackForce).Value;
            enemy.AddEffect(knockback.InitializeEffect(enemy, new ForceData(Game.PSystems.PlayerObject.transform.position, force, 0)));

            foreach (var trinket in Game.PSystems.CurrentTrinkets)
            {
                if (trinket is OnHitEffectTrinketSO)
                {
                    var OHTrinket = (OnHitEffectTrinketSO)trinket;
                    OHTrinket.OnHitAction(enemy);
                }
            }
        }

    }
    private void ShowGraphics()
    {
        isRight = Game.PSystems.AnimationController.IsRight;

        var z = 180;
        if (isRight)
            z = 0;

        var sourceAngles = Source.rotation.eulerAngles;
        var tmpAngles = sourceAngles;
        tmpAngles.z = z;

        var cone = Instantiate(_damageCone, Source.position, Quaternion.Euler(tmpAngles), Source);
        var coneControl = cone.GetComponent<IConeControl>();
        if (coneControl == null)
            return;

        cone.SetActive(false);
        coneControl.Radius = WeaponData.GetStat(StatParam.AttackRange).Value;
        coneControl.Angle = WeaponData.GetStat(StatParam.AttackCone).Value;
        cone.SetActive(true);
    }

    private List<Vector3> GetSectorCirclePoints(Vector3 enemyPos, float enemyRadius, Vector3 sourcePos, Vector3 forward, float angle, float distance)
    {

        Vector3 sectorStart = Quaternion.Euler(0, -angle / 2, 0) * forward * angle + sourcePos;
        Vector3 sectorEnd = Quaternion.Euler(0, angle / 2, 0) * forward * angle + sourcePos;

        var startLinePoints = GetCircleLineIntersections(enemyPos, enemyRadius, sourcePos, sectorStart, distance);
        var endLinePoints = GetCircleLineIntersections(enemyPos, enemyRadius, sourcePos, sectorEnd, distance);

        List<Vector3> points = new List<Vector3>();
        points.AddRange(startLinePoints);
        points.AddRange(endLinePoints);
        return points;
    }
    private List<Vector3> GetCircleLineIntersections(Vector3 P1, float R1, Vector3 lineStart, Vector3 lineEnd, float distance)
    {
        Vector3 intersection1 = Vector3.zero;
        Vector3 intersection2 = Vector3.zero;

        // Calculate the direction vector of the line
        Vector3 dir = (lineEnd - lineStart).normalized;

        // Calculate the vector from the circle's center to the line's start point
        Vector3 circleToLineStart = lineStart - P1;

        // Calculate the coefficients for the quadratic equation
        float a = Vector3.Dot(dir, dir);
        float b = 2 * Vector3.Dot(circleToLineStart, dir);
        float c = Vector3.Dot(circleToLineStart, circleToLineStart) - R1 * R1;

        b = Mathf.Clamp(b, float.NegativeInfinity, 0);


        // Calculate the discriminant
        float discriminant = b * b - 4 * a * c;

        // If the discriminant is negative, there are no intersections
        if (discriminant < 0)
        {
            return new List<Vector3>();
        }

        // Calculate the two possible solutions for t
        float t1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
        float t2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);

        // Calculate the intersection points
        intersection1 = lineStart + t1 * dir;
        intersection2 = lineStart + t2 * dir;

        //Debug.Log($"T1: {t1}");
        //Debug.Log($"T2: {t2}");


        List<Vector3> intersectionPoints = new List<Vector3>();

        if (Vector3.Distance(lineStart, intersection1) < distance)
            intersectionPoints.Add(intersection1);
        if (Vector3.Distance(lineStart, intersection2) < distance)
            intersectionPoints.Add(intersection2);
        return intersectionPoints;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Source.position, WeaponData.GetStat(StatParam.AttackRange).Value);
    }
}
