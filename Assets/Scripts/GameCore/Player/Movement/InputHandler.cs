using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public Vector3 MousePosition { get; set; }

    // Start is called before the first frame update
    public bool SwapWeapon = false;
    public bool UseAbility = false;

    public float Horizontal;
    public float Vertical;
    private float _moveAmount;

    private PlayerControls _inputActions;
    private Vector2 _movementInput;

    public void OnEnable()
    {
        if (_inputActions == null)
        {
            _inputActions = new PlayerControls();
            _inputActions.PlayerMovement.Movement.performed += inputActions => _movementInput = inputActions.ReadValue<Vector2>();
        }
        _inputActions.Enable();
    }

    public void OnDisable()
    {
        _inputActions.Disable();
    }
    public void TickInput(float delta)
    {
        MoveInput(delta);
        MousePosition = Input.mousePosition;


        if (Input.GetKeyDown("q"))
            SwapWeapon = true;
        else
            SwapWeapon = false;

        if (Input.GetKeyDown("space"))
            UseAbility = true;
        else
            UseAbility = false;
    }
    private void MoveInput(float delta)
    {
        Horizontal = _movementInput.x;
        Vertical = _movementInput.y;
        _moveAmount = Mathf.Clamp01(Mathf.Abs(Horizontal) + Mathf.Abs(Vertical));
    }
}
