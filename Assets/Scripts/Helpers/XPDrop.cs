using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using Random = UnityEngine.Random;

public class XPDrop : MonoBehaviour
{

    const int TEAL = 0;
    const int JADE = 1;

    public float XpValue;
    public string ID;
    private Transform _myTransform;
    private Transform _target;

    [SerializeField] private float _speed;
    private float _minSpeed = 0.01f;
    private float _maxSpeed = 0.15f;
    private float _stepInc = 0.02f;

    private Material _mat;

    TweenerCore<Vector3, Vector3, VectorOptions> _hoverTween;
    TweenerCore<Vector3, Vector3, VectorOptions> _inBounce;
    TweenerCore<Quaternion, Vector3, QuaternionOptions> _rotateTween;

    private List<Tuple<Color, Color, Color>> _gradients = new List<Tuple<Color, Color, Color>>() //Top, Bottom
    {
        new Tuple<Color, Color, Color>(new Color32(0,225,255,255),new Color32(0,60,175,255),new Color32(5,55,125,255)), // Teal
        new Tuple<Color, Color, Color>(new Color32(0,255,50,255),new Color32(0,95,50,255),new Color32(15,125,50,255))  // Jade
    };

    // Start is called before the first frame update
    void Awake()
    {
        _mat = GetComponentInChildren<MeshRenderer>().material;
        ID = Globals.GenerateId();
        XpValue = Random.Range(0.5f, 1.5f);

        if (XpValue > 1.2f)
        {
            _mat.SetColor("_Gradient_Top", _gradients[JADE].Item1);
            _mat.SetColor("_Gradient_Bottom", _gradients[JADE].Item2);
            _mat.SetColor("_Flaps_color", _gradients[JADE].Item3);
        }
        else
        {
            _mat.SetColor("_Gradient_Top", _gradients[TEAL].Item1);
            _mat.SetColor("_Gradient_Bottom", _gradients[TEAL].Item2);
            _mat.SetColor("_Flaps_color", _gradients[TEAL].Item3);
        }

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
        if (Game.PlayerInMenu == true)
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
