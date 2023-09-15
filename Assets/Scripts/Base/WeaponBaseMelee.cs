using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBaseMelee : WeaponBase
{
    Vector3 edgePoint = Vector3.zero;
    List<Vector3> hits = new List<Vector3>();
    [SerializeField] private GameObject _damageArc;
    protected override void Attack()
    {
        var go = Instantiate(_damageArc, Source.position, Source.rotation, Source);

        var forward = Source.forward;
        var arcValue = Mathf.Cos((WeaponData.AttackArc.Value / 2) * Mathf.Deg2Rad);
        var sourceFloored = new Vector3(Source.position.x, 0, Source.position.z);

        Collider[] hitEnemies = Physics.OverlapSphere(sourceFloored, WeaponData.AttackRange.Value, WeaponData.EnemyLayer);

        foreach (var hitEnemy in hitEnemies)
        {
            base.Attack();

            var capsule = ((CapsuleCollider)hitEnemy);
            var maxVectorValue = Globals.GetLargestValue(capsule.gameObject.transform.localScale, true);
            var realColRadius = capsule.radius * maxVectorValue;


            Vector3 enemyPos = hitEnemy.transform.position;
            Vector3 enemyPosFloored = new Vector3(enemyPos.x, 0, enemyPos.z);
            Vector3 dirSrcToEnemy = (enemyPosFloored - sourceFloored).normalized;

            var dot = Vector3.Dot(dirSrcToEnemy, forward);
            var d = GetSectorCirclePoints(enemyPosFloored, realColRadius, sourceFloored, forward, WeaponData.AttackArc.Value, WeaponData.AttackRange.Value);

            //Debug.DrawRay(sourceFloored, dirSrcToEnemy, Color.cyan, 1f);

            //hits.Add(sourceFloored);
            //hits.Add(enemyPosFloored);
            //hits.AddRange(d);

            //var centerDotProduct = Vector3.Dot(dirSrcToEnemy, forward); //1 = right in front, -1 = right behind

            if (dot >= arcValue)
            {
                Heat.AddHeat(1);
                var enemy = hitEnemy.GetComponent<NewEnemyBase>();
                enemy.Damage(WeaponData.AttackDamage.Value);
            }

            else if (d.Count > 0)
            {
                Heat.AddHeat(1);
                var enemy = hitEnemy.GetComponent<NewEnemyBase>();
                enemy.Damage(WeaponData.AttackDamage.Value);
            }

            //enemyPos * player.transform.right * collider.radius;
        }
    }

    List<Vector3> GetSectorCirclePoints(Vector3 enemyPos, float enemyRadius, Vector3 sourcePos, Vector3 forward, float angle, float distance)
    {

        Vector3[] intersectionPoints = new Vector3[0];
        Vector3 sectorStart = Quaternion.Euler(0, -angle / 2, 0) * forward * angle + sourcePos;
        Vector3 sectorEnd = Quaternion.Euler(0, angle / 2, 0) * forward * angle + sourcePos;
        Vector3 v2 = Quaternion.Euler(0, 0, 0) * forward * angle + sourcePos;

        //Debug.DrawLine(sourcePos, sectorStart, Color.blue, 1f);
        //Debug.DrawLine(sourcePos, sectorEnd, Color.blue, 1f);

        //GetCircleLineIntersections(enemyPos, enemyRadius, sourcePos, v2, out a, out b);

        var startLinePoints = GetCircleLineIntersections(enemyPos, enemyRadius, sourcePos, sectorStart, distance);
        var endLinePoints = GetCircleLineIntersections(enemyPos, enemyRadius, sourcePos, sectorEnd, distance);
        List<Vector3> points = new List<Vector3>();
        points.AddRange(startLinePoints);
        points.AddRange(endLinePoints);
        return points;
    }
    public List<Vector3> GetCircleLineIntersections(Vector3 P1, float R1, Vector3 lineStart, Vector3 lineEnd, float distance)
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



        //Debug.Log($"A: {a}");
        //Debug.Log($"B: {b}");
        //Debug.Log($"C: {c}");
        //Debug.Log($"Disc: {discriminant}");

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

        foreach (var hit in hits)
        {
            if (hit != Vector3.zero)
            {
                Gizmos.DrawSphere(hit, .1f);
            }
        }

        /*
        if (edgePoint != Vector3.zero)
        {
            //Gizmos.DrawSphere(edgePoint, .3f);
        }

        foreach (var hit in hits)
        {
            if (hit != Vector3.zero)
            {
                Gizmos.DrawSphere(hit, .3f);
            }
        }
        Gizmos.DrawWireSphere(Source.position, WeaponData.AttackRange.Value);
        //Gizmos.DrawLine(Globals.PlayerTransform.position, Globals.PlayerTransform.forward * 100);
        */
    }
}
