using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Animator _anim;
    private float _animRatio = 0.43f;
    // Start is called before the first frame update
    void Awake()
    {
        Globals.PAnimationController = this;
        _anim = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        Globals.PSystems.OnAttack += OnAttack;
        Globals.PMovementController.OnMove += OnMove;
    }

    private void OnAttack()
    {
        _anim.SetTrigger("Attack");
    }
    private void OnMove(Vector3 moveDirection, float speed)
    {
        var VAnim = Vector3.Dot(Globals.PlayerTransform.forward, moveDirection.normalized);
        var HAnim = Vector3.Dot(Globals.PlayerTransform.right, moveDirection.normalized);

        _anim.SetFloat("Horizontal", HAnim);
        _anim.SetFloat("Vertical", VAnim);
        _anim.SetFloat("Speed", speed * _animRatio);
    }
}
