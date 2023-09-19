using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swoosher : MonoBehaviour
{
    [SerializeField] private Material _swooshMat;
    [SerializeField] private Material _swooshTipMat;
    private GameObject swoosher;
    [SerializeField] private AnimationCurve _swooshCurve;

    private List<Transform> smallPoints = new List<Transform>();
    private List<Transform> bigPoints = new List<Transform>();

    float timeMin = 0.05f;
    float timeMax = 0.2f;

    // Start is called before the first frame update
    public GameObject BuildSwoosher(Transform parent, Transform source, float length, float angle)
    {
        swoosher = new GameObject("SwoosherPivot");
        swoosher.transform.SetParent(parent);
        swoosher.transform.position = source.position;
        swoosher.transform.localEulerAngles = new Vector3(0, -1 * angle / 2, 0);


        int trailCount = Mathf.RoundToInt(length);
        int bigTrailCount = Mathf.RoundToInt(length / 2);
        float offset = 0;

        float spacingLength = length - offset;
        float spacePart = spacingLength / trailCount;
        float spaceMargin = spacePart / 5;


        float spacePartSec = spacingLength / bigTrailCount;
        float spaceMarginSec = spacePartSec / 3;

        float start = offset;
        while (start < length)
        {
            float tp;
            tp = Random.Range(start + spaceMargin, start + spaceMargin + spaceMargin * 3);
            start = tp + spaceMargin;
            if (start >= length)
                break;
            var swooshPoint = new GameObject("Swooshpoint");
            swooshPoint.transform.position = swoosher.transform.position + (spaceMargin + tp) * swoosher.transform.forward;
            swooshPoint.transform.SetParent(swoosher.transform);
            var tr = swooshPoint.AddComponent<TrailRenderer>();
            tr.time = Random.Range(timeMin, timeMax);
            tr.material = _swooshMat;
            tr.widthCurve = _swooshCurve;
            for (int i = 0; i < tr.widthCurve.keys.Length; i++)
            {
                var c = tr.widthCurve.keys[i];
                c.value = Random.Range(c.value - 0.1f, c.value + 0.1f);
            }
            smallPoints.Add(swooshPoint.transform);
        }
        start = offset;
        while (start < length)
        {
            float tp;
            tp = Random.Range(start + spaceMarginSec, start + spaceMarginSec + spaceMarginSec);
            start = tp + spaceMarginSec;
            if (start >= length)
                break;
            var swooshPoint = new GameObject("Swooshpoint");
            swooshPoint.transform.position = swoosher.transform.position + (spaceMarginSec + tp) * swoosher.transform.forward;
            swooshPoint.transform.SetParent(swoosher.transform);
            var tr = swooshPoint.AddComponent<TrailRenderer>();
            tr.time = Random.Range(timeMin, timeMax);
            tr.material = _swooshTipMat;
            tr.widthCurve = _swooshCurve;
            for (int i = 0; i < tr.widthCurve.keys.Length; i++)
            {
                var c = tr.widthCurve.keys[i];
                c.value = Random.Range((c.value + 0.5f) - 0.1f, (c.value + 0.5f) + 0.1f);
            }
            bigPoints.Add(swooshPoint.transform);
        }


        var swooshTip = new GameObject("Swooshpoint");
        var trT = swooshTip.AddComponent<TrailRenderer>();
        trT.time = Random.Range(timeMin, timeMax);
        trT.material = _swooshTipMat;
        trT.widthCurve = _swooshCurve;

        swooshTip.transform.position = swoosher.transform.position + (length - trT.startWidth / 2) * swoosher.transform.forward;
        swooshTip.transform.SetParent(swoosher.transform);

        return swoosher;

        //Sequence s = DOTween.Sequence();
        //s.SetDelay(1);
        //s.Append(_swoosher.transform.DORotate(new Vector3(0, 45, 0), .1f).SetEase(Ease.Linear));
        //s.AppendInterval(1f);
        //s.SetLoops(-1, LoopType.Yoyo);
        //s.Play();
    }
}
