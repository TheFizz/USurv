using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public event MovementAnimHandler OnMove;

    [SerializeField] private Animator _charAnimator;
    [SerializeField] private LayerMask _mouseHitMask;
    private Rigidbody _RB;
    private Collider _collider;
    public TextMeshProUGUI debugSpeedText;

    public bool lockPosition = false, lockRotation = false;
    private void Start()
    {
        _RB = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }
    public void Update()
    {
        float delta = Time.deltaTime;
        Game.InputHandler.TickInput(delta);

        if (!lockPosition)
            HandleMovement();
        if (!lockRotation)
            HandleRotation();

    }
    private void HandleRotation()
    {
        Ray ray = Game.MainCamera.ScreenPointToRay(Game.InputHandler.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitinfo, layerMask: _mouseHitMask, maxDistance: 300f))
        {
            var targetLook = hitinfo.point;
            targetLook.y = transform.position.y;
            transform.LookAt(targetLook);
        }
    }
    private void HandleMovement()
    {
        Vector3 moveDirection;

        moveDirection = Vector3.forward * Game.InputHandler.Vertical;
        moveDirection += Vector3.right * Game.InputHandler.Horizontal;


        moveDirection = Quaternion.Euler(0, Game.MainCamera.gameObject.transform.eulerAngles.y, 0) * moveDirection;


        moveDirection.Normalize();

        var speed = Game.PSystems.PlayerData.GetStat(StatParam.PlayerMoveSpeed).Value;
        OnMove?.Invoke(moveDirection, speed);
        moveDirection *= speed;

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, Vector3.up);
        _RB.velocity = projectedVelocity;
    }
    public void ForceMoveGhost(Vector3 force, float time)
    {
        StartCoroutine(ForceMoveGhostCR(force, time));
    }
    IEnumerator ForceMoveGhostCR(Vector3 force, float time)
    {
        lockPosition = true;
        _collider.enabled = false;
        _RB.velocity = force;
        yield return new WaitForSeconds(time);
        _RB.velocity = Vector3.zero;
        _collider.enabled = true;
        lockPosition = false;
    }
}
