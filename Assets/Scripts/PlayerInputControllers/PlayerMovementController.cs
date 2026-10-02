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
    [SerializeField] private float waterDrag = 2f;
    [SerializeField] private float airDrag = 0.1f;

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference verticalAction;
    [SerializeField] private InputActionReference sprintAction;

    private bool inWater = false;

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
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (!IsOwner) 
            return;
        moveAction.action.Disable();
        verticalAction.action.Disable();
        sprintAction.action.Disable();
    }

    private void Update()
    {
        if (!IsOwner) 
            return;
        RotateVisual();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) 
            return;
        Move();
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

    public void SetInWater(bool isInWater)
    {
        inWater = isInWater;
        if (IsOwner)
            ApplyMedium();   
    }
}