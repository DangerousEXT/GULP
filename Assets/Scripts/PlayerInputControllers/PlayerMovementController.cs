using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : NetworkBehaviour
{
    [Header("Refs")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform cameraRoot;

    [Header("Swim")]
    [SerializeField] private float swimSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;
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
        if (!IsOwner)
        {
            if (rb != null) rb.isKinematic = false;
            return;
        }

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.linearDamping = waterDrag;
        rb.angularDamping = 5f;
        rb.freezeRotation = true;

        moveAction.action.Enable();
        verticalAction.action.Enable();
        sprintAction.action.Enable();
    }

    public override void OnStopClient()
    {
        if (!IsOwner) return;

        moveAction.action.Disable();
        verticalAction.action.Disable();
        sprintAction.action.Disable();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        var input = moveAction.action.ReadValue<Vector2>();
        var vertical = verticalAction.action.ReadValue<float>();
        var sprint = sprintAction.action.IsPressed();

        var forward = cameraRoot.forward;
        var right = cameraRoot.right;

        var moveDir = forward * input.y + right * input.x;

        moveDir += Vector3.up * vertical;

        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        var speed = sprint ? sprintSpeed : swimSpeed;

        var targetVelocity = moveDir * speed;
        rb.linearVelocity = Vector3.Lerp(
            rb.linearVelocity,
            targetVelocity,
            Time.fixedDeltaTime * acceleration
        );
    }

    public void SetInWater(bool isInWater)
    {
        inWater = isInWater;
        if (rb != null)
            rb.linearDamping = inWater ? waterDrag : airDrag;
    }
}