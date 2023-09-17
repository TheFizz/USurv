using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class XPDrop : MonoBehaviour
{
    public float XpValue;
    public string ID;
    private Transform _myTransform;
    private Transform _target;

    [SerializeField] private float _speed;
    private float _minSpeed = 0.01f;
    private float _maxSpeed = 0.1f;
    private float _stepInc = 0.02f;

    TweenerCore<Vector3, Vector3, VectorOptions> _hoverTween;
    TweenerCore<Vector3, Vector3, VectorOptions> _inBounce;
    TweenerCore<Quaternion, Vector3, QuaternionOptions> _rotateTween;

    // Start is called before the first frame update
    void Awake()
    {
        ID = Globals.GenerateId();
        XpValue = Random.Range(0.5f, 1.5f);

        float loopTime = Random.Range(1f, 2f);
        bool ccwRotation = Random.Range(0, 2) == 1;
        float rotationDegrees = 90;

        if (ccwRotation)
            rotationDegrees *= -1;

        _myTransform = GetComponent<Transform>();

        _hoverTween = _myTransform.DOMoveY(0.2f, loopTime).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        _rotateTween = _myTransform.DOLocalRotate(new Vector3(0, rotationDegrees, 0), loopTime).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);

        gameObject.name = $"XPDrop<{ID}>";

        Globals.XPDropsPool.Add(ID, this);
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
    public void Destroy()
    {
        _hoverTween.Kill();
        _rotateTween.Kill();

        _inBounce = _myTransform.DOScale(0, 0.1f).SetEase(Ease.OutBounce).OnComplete(() =>
         {
             Globals.XPDropsPool.Remove(ID);
             Destroy(gameObject);
         }
        );
    }
    private void OnDestroy()
    {
        _inBounce.Kill();
    }
}
