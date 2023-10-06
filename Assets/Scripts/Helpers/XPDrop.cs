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

    public DropAnimator Animator;

    TweenerCore<Vector3, Vector3, VectorOptions> _hoverTween;
    TweenerCore<Vector3, Vector3, VectorOptions> _inBounce;
    TweenerCore<Quaternion, Vector3, QuaternionOptions> _rotateTween;

    private List<Tuple<Color, Color, Color>> _gradients = new List<Tuple<Color, Color, Color>>() //Top, Bottom
    {
        new Tuple<Color, Color, Color>(new Color32(0,225,255,255),new Color32(0,60,175,255),new Color32(5,55,125,255)), // Teal
        new Tuple<Color, Color, Color>(new Color32(0,255,50,255),new Color32(0,95,50,255),new Color32(15,125,50,255))  // Jade
    };

    private void Awake()
    {
        _myTransform = transform;
        ID = Game.GenerateId();
        gameObject.name = $"XPDrop<{ID}>";
        Game.XPDropsPool.Add(ID, this);

        _mat = GetComponentInChildren<MeshRenderer>().material;
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

    }
    private void Update()
    {
        if (_target == null)
            return;
        if (Game.Room.PlayerInMenu == true)
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
