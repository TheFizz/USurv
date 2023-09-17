using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class XPDrop : MonoBehaviour
{
    public float XpValue;
    private Transform _myTransform;
    private Transform _target;

    [SerializeField] private float _speed;
    private float _minSpeed = 0.01f;
    private float _maxSpeed = 0.1f;
    private float _stepInc = 0.02f;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Drop");
        XpValue = Random.Range(1f, 3f);

        float loopTime = Random.Range(1f, 2f);
        bool ccwRotation = Random.Range(0, 2) == 1;
        float rotationDegrees = 90;

        if (ccwRotation)
            rotationDegrees *= -1;

        _myTransform = GetComponent<Transform>();
        _myTransform.DOMoveY(0.2f, loopTime).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        _myTransform.DOLocalRotate(new Vector3(0, rotationDegrees, 0), loopTime).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }
    private void Update()
    {
        if (_target == null)
            return;
        _speed = Mathf.Lerp(_speed, _maxSpeed, _stepInc);

        _myTransform.position = Vector3.MoveTowards(_myTransform.position, _target.position, _speed);
    }
    public void Attract(Transform target)
    {
        if (_target != null)
            return;

        _target = target;
        _speed = _minSpeed;
    }
}
