using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private LayerMask _mouseHitMask;
    private InputHandler _input;
    private Rigidbody _RB;
    private float _moveSpeed;

    public bool lockPosition = false, lockRotation = false;
    void Awake()
    {
        _input = Globals.Input;
        _RB = GetComponent<Rigidbody>();
    }
    public void Update()
    {
        if (!lockPosition)
            HandleMovement();
        if (!lockRotation)
            HandleRotation();

    }
    private void HandleRotation()
    {
        Ray ray = Globals.MainCamera.ScreenPointToRay(_input.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitinfo, layerMask: _mouseHitMask, maxDistance: 300f))
        {
            var targetLook = hitinfo.point;
            targetLook.y = transform.position.y;
            transform.LookAt(targetLook);
        }
    }
    private void HandleMovement()
    {
        float delta = Time.deltaTime;
        _input.TickInput(delta);
        Vector3 moveDirection;

        moveDirection = Vector3.forward * _input.Vertical;
        moveDirection += Vector3.right * _input.Horizontal;
        moveDirection = Quaternion.Euler(0, Globals.MainCamera.gameObject.transform.eulerAngles.y, 0) * moveDirection;
        moveDirection.Normalize();

        float speed = _moveSpeed;
        moveDirection *= speed;

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, Vector3.up);
        _RB.velocity = projectedVelocity;
    }
    public void SetMoveSpeed(float moveSpeed)
    {
        _moveSpeed = moveSpeed;
    }
}
