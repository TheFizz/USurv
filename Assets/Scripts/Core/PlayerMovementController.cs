using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Camera _mainCam;
    [SerializeField] private LayerMask _mouseRayReceiver;
    private InputHandler _input;
    private Rigidbody _RB;

    // Start is called before the first frame update
    void Awake()
    {
        _input = GetComponent<InputHandler>();
        _RB = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        MoveToTarget();
        RotateToTarget();
    }

    private void RotateToTarget()
    {
        Ray ray = _mainCam.ScreenPointToRay(_input.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitinfo, layerMask: _mouseRayReceiver, maxDistance: 300f))
        {
            var targetLook = hitinfo.point;
            targetLook.y = transform.position.y;
            transform.LookAt(targetLook);
            Debug.DrawLine(transform.position, targetLook, Color.red);
        }
    }

    void MoveToTarget()
    {
        _RB.velocity = Vector3.zero;
        var targetMove = new Vector3(_input.InputVector.x, 0, _input.InputVector.y);
        //Modify the vector to account for camera angle
        targetMove = Quaternion.Euler(0, _mainCam.gameObject.transform.eulerAngles.y, 0) * targetMove;
        var speed = _moveSpeed * Time.deltaTime;
        _RB.velocity = targetMove * _moveSpeed;

        //var targetPosition = transform.position + targetMove * speed;
        //rb.MovePosition(targetPosition);
        //transform.position = targetPosition;
    }
}
