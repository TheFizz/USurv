using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    Animator _anim;
    // Start is called before the first frame update
    void Awake()
    {
        Globals.CharacterAnimator = this;
        _anim = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        Globals.PSystems.OnAttack += OnAttack;
    }

    private void OnAttack()
    {
        _anim.SetTrigger("Attack");
    }
}
