using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwapMode
{
    Rotate,
    TwoKey
}

public class InputHandler : MonoBehaviour
{
    public SwapMode swapMode = SwapMode.Rotate;
    public Vector3 MousePosition { get; set; }
    private bool _inputEnabled = true;

    // Start is called before the first frame update
    public bool SwapWeapon = false;
    public bool UseAbility = false;


    public bool Swap01 = false;
    public bool Swap12 = false;

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
        if (!_inputEnabled)
            return;

        MoveInput(delta);
        MousePosition = Input.mousePosition;

        if (Input.GetKeyDown("space"))
            UseAbility = true;
        else
            UseAbility = false;

        if (Input.GetKeyDown("r"))
            switch (swapMode)
            {
                case SwapMode.Rotate:
                    swapMode = SwapMode.TwoKey;
                    break;
                case SwapMode.TwoKey:
                    swapMode = SwapMode.Rotate;
                    break;
                default:
                    break;
            }

        if (Input.GetMouseButtonDown(0))
            switch (swapMode)
            {
                case SwapMode.Rotate:
                    SwapWeapon = true;
                    break;
                case SwapMode.TwoKey:
                    Swap01 = true;
                    break;
                default:
                    break;
            }
        else
        {
            SwapWeapon = false;
            Swap01 = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (swapMode == SwapMode.TwoKey)
            {
                Swap12 = true;
            }
        }
        else
            Swap12 = false;
    }
    private void MoveInput(float delta)
    {
        Horizontal = _movementInput.x;
        Vertical = _movementInput.y;
        _moveAmount = Mathf.Clamp01(Mathf.Abs(Horizontal) + Mathf.Abs(Vertical));
    }
    public void SetInputEnabled(bool enabled)
    {
        _inputEnabled = enabled;
    }
}
