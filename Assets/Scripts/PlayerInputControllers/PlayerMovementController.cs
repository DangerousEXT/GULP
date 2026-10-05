using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : NetworkBehaviour
{
    [Header("Refs")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform cameraRoot;

    [Header("Visual Rotation")]
    [SerializeField] private Transform visual;
    [SerializeField] private float rotationSpeed = 7f;

    [Header("Ground")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 20f;

    [Header("Swim")]
    [SerializeField] private float swimSpeed = 10f;
    [SerializeField] private float sprintSpeed = 20f;

    [SerializeField] private float verticalSpeed = 3f;
    [SerializeField] private float acceleration = 10f;

    [Header("Water")]
    [SerializeField] private float waterDrag = 0.5f;
    [SerializeField] private float airDrag = 0.1f;
    [Range(0f, 1f)] 
    [SerializeField] private float waterGravityScale = 0.4f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundMask;

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference verticalAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference jumpAction;

    private bool inWater = false;
    private bool isGrounded;
    private bool jumpQueued;

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log($"[Player] objId={ObjectId} owner={IsOwner} ownerId={OwnerId} name={name} scene={gameObject.scene.name}/{gameObject.scene.handle}");
        if (!IsOwner)
        {
            rb.isKinematic = true;
            return;
        }
        rb.isKinematic = false;
        rb.angularDamping = 5f;
        rb.freezeRotation = true;
        ApplyMedium();
        moveAction.action.Enable();
        verticalAction.action.Enable();
        sprintAction.action.Enable();
        jumpAction.action.Enable();
        jumpAction.action.performed += OnJumpPerformed;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (!IsOwner) 
            return;
        moveAction.action.Disable();
        verticalAction.action.Disable();
        sprintAction.action.Disable();
        jumpAction.action.Disable();
    }

    private void Update()
    {
        if (!IsOwner) 
            return;
        RotateVisual();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) 
            return;
        Move();
        ApplyJump();
        ApplyWaterGravity();
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        if (isGrounded && !inWater)
            jumpQueued = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void ApplyJump()
    {
        if (!jumpQueued)
            return;
        jumpQueued = false;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }

    private void Move()
    {
        var input = moveAction.action.ReadValue<Vector2>();
        var vertical = verticalAction.action.ReadValue<float>();
        var sprint = sprintAction.action.IsPressed();
        var moveDir = cameraRoot.forward * input.y + cameraRoot.right * input.x;
        Vector3 target;

        if (inWater)
        {
            moveDir += Vector3.up * vertical;
            if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();
            target = moveDir * (sprint ? sprintSpeed : swimSpeed);
        }
        else
        {
            moveDir.y = 0f;
            if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();
            target = moveDir * (sprint ? runSpeed : walkSpeed);
            target.y = rb.linearVelocity.y;
        }

        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, target, Time.fixedDeltaTime * acceleration);
    }

    private void RotateVisual()
    {
        var targetYaw = cameraRoot.eulerAngles.y;
        var t = 1f - Mathf.Exp(-rotationSpeed * Time.deltaTime);
        var yaw = Mathf.LerpAngle(visual.eulerAngles.y, targetYaw, t);
        visual.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    private void ApplyMedium()
    {
        rb.useGravity = !inWater;
        rb.linearDamping = inWater ? waterDrag : airDrag;
    }

    private void ApplyWaterGravity()
    {
        if (!inWater) 
            return;
        rb.AddForce(Physics.gravity * waterGravityScale, ForceMode.Acceleration);
    }

    public void SetInWater(bool isInWater)
    {
        inWater = isInWater;
        if (IsOwner)
        {
            ApplyMedium();
        }
    }
}