using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputHandler : MonoBehaviour
{

    [HideInInspector] public SwapMode swapMode = SwapMode.TwoKey;
    [HideInInspector] public Vector3 MousePosition { get; set; }
    private bool _inputEnabled = true;
    private bool _mouseInputEnabled = true;

    // Start is called before the first frame update
    [HideInInspector] public bool SwapWeapon = false;
    [HideInInspector] public bool UseAbility = false;


    [HideInInspector] public bool Swap01 = false;
    [HideInInspector] public bool Swap12 = false;

    [HideInInspector] public float Horizontal;
    [HideInInspector] public float Vertical;
    private float _moveAmount;

    private PlayerControls _inputActions;
    private Vector2 _movementInput;


    public event Action<int> OnTabPress;
    public event Action<int> OnTabRelease;

    public void OnEnable()
    {
        Game.InputHandler = this;

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

        if (Input.GetKey(KeyCode.Tab))
        {
            Time.timeScale = 0;
            Game.PSystems.SetPlayerLocked(true);
            OnTabPress?.Invoke(1);
        }

        else
        {  
            Time.timeScale = 1;
            Game.PSystems.SetPlayerLocked(false);
            OnTabRelease?.Invoke(1);
        }

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

        if (!_mouseInputEnabled)
            return;

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
    public void SetMouseInputEnabled(bool enabled)
    {
        _mouseInputEnabled = enabled;
    }
}
