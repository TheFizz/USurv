using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerAnimationController : MonoBehaviour
{
    public Ease WeaponEase;
    [SerializeField] GameObject _wpnPf;
    private GameObject _wpnObj;
    private Transform _wpnModel;
    Animator _anim;
    Animator _wpnAnim;
    private Transform _source;
    private float _animRatio = 0.43f;
    [SerializeField] private float _angVel = 0.1f; //200f;
    [SerializeField] private bool weaponAnimation = true;

    [HideInInspector] public bool IsRight = false;

    // Start is called before the first frame update
    void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
    }
    private void Activate()
    {
        _wpnObj = Instantiate(_wpnPf, _source.position, Quaternion.identity, Game.PSystems.PlayerObject.transform);
        _wpnObj.SetActive(false);
        _wpnModel = _wpnObj.transform.GetChild(0);
        _wpnAnim = _wpnObj.GetComponentInChildren<Animator>();

        Game.PSystems.OnAttack += OnAttack;
        Game.PSystems.MovementController.OnMove += OnMove;
    }

    private void OnAttack(float range, float cone)
    {
        _anim.SetTrigger("Attack");
        if (!weaponAnimation)
            return;

        int dirMult = 1;
        int flipDeg = 0;
        if (IsRight)
        {
            dirMult = -1;
            flipDeg = 180;
        }

        var fromRot = new Vector3(0, (cone + 30) / (2 * dirMult), flipDeg);
        var toRot = new Vector3(0, (cone) / (-2 * dirMult), flipDeg);

        DOTween.Kill(_wpnObj.transform);
        _wpnObj.SetActive(true);
        _wpnObj.transform.localEulerAngles = fromRot;
        _wpnModel.position = _source.position + (_wpnObj.transform.forward * range);
        //_wpnObj.transform.DOLocalRotate(toRot, _angVel).SetSpeedBased(true).SetEase(WeaponEase).OnComplete(() => _wpnObj.SetActive(false));
        _wpnObj.transform.DOLocalRotate(toRot, _angVel).SetEase(WeaponEase).OnComplete(() => _wpnObj.SetActive(false));

        IsRight = !IsRight;
    }
    private void OnMove(Vector3 moveDirection, float speed)
    {
        var VAnim = Vector3.Dot(Game.PSystems.PlayerObject.transform.forward, moveDirection.normalized);
        var HAnim = Vector3.Dot(Game.PSystems.PlayerObject.transform.right, moveDirection.normalized);

        _anim.SetFloat("Horizontal", HAnim);
        _anim.SetFloat("Vertical", VAnim);
        _anim.SetFloat("Speed", speed * _animRatio);
    }

    internal void SetSource(Transform source)
    {
        _source = source;
        Activate();
    }
    private void OnDestroy()
    {
        Game.PSystems.OnAttack -= OnAttack;
        Game.PSystems.MovementController.OnMove -= OnMove;
        DOTween.Kill(_wpnObj.transform);
    }
}
