using TMPro;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public event MovementAnimHandler OnMove;

    [SerializeField] private Animator _charAnimator;
    [SerializeField] private LayerMask _mouseHitMask;
    private Rigidbody _RB;
    public TextMeshProUGUI debugSpeedText;

    public bool lockPosition = false, lockRotation = false;
    void Awake()
    {
        Globals.PMovementController = this;
        Globals.PlayerTransform = transform;
    }
    private void Start()
    {
        Globals.PSystems.SetSource(Globals.PlayerTransform.Find("AttackSource").gameObject.transform);
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
        Ray ray = Globals.MainCamera.ScreenPointToRay(Globals.InputHandler.MousePosition);
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
        Globals.InputHandler.TickInput(delta);
        Vector3 moveDirection;

        moveDirection = Vector3.forward * Globals.InputHandler.Vertical;
        moveDirection += Vector3.right * Globals.InputHandler.Horizontal;


        moveDirection = Quaternion.Euler(0, Globals.MainCamera.gameObject.transform.eulerAngles.y, 0) * moveDirection;


        moveDirection.Normalize();

        var speed = Globals.PSystems.PlayerData.GetStat(StatParam.PlayerMoveSpeed).Value;
        OnMove?.Invoke(moveDirection, speed);
        moveDirection *= speed;

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, Vector3.up);
        _RB.velocity = projectedVelocity;

    }
}
