using FishNet.Connection;
using FishNet.Object;
using NUnit.Framework.Internal.Execution;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractsWithItemsController : NetworkBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference takeAction;

    [Header("Refs")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform holdPoint;

    [Header("Pickup")]
    [SerializeField] private float range = 3f;
    [SerializeField] private float radius = 0.15f;
    [SerializeField] private LayerMask pickupMask;

    [Header("Follow")]
    [SerializeField] private float followSpeed = 15f;

    private Item heldItem;

    public override void OnStartClient()
    {
        if (!IsOwner) return;

        takeAction.action.Enable();
        takeAction.action.performed += OnTakePerformed;
        takeAction.action.canceled += OnTakeCanceled;
    }

    public override void OnStopClient()
    {
        if (!IsOwner) return;

        takeAction.action.performed -= OnTakePerformed;
        takeAction.action.canceled -= OnTakeCanceled;
        takeAction.action.Disable();
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (heldItem != null) UpdateHeldPosition();
    }

    private void OnTakePerformed(InputAction.CallbackContext ctx)
    {
        if (heldItem == null)
            TryTake();
    }

    private void OnTakeCanceled(InputAction.CallbackContext ctx)
    {
        if (heldItem != null)
            Drop();
    }

    private void TryTake()
    {
        var ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        bool hitSomething = Physics.SphereCast(
            ray,
            radius,
            out RaycastHit hit,
            range,
            pickupMask,
            QueryTriggerInteraction.Ignore
        );

        if (!hitSomething) return;
        if (!hit.collider.TryGetComponent(out Item item)) return;
        

        heldItem = item;
    }

    private void Drop()
    {
        heldItem = null;
    }

    private void UpdateHeldPosition()
    {
        var targetPos = holdPoint.position;
        var targetRot = mainCamera.transform.rotation;

        heldItem.transform.position = Vector3.Lerp(
            heldItem.transform.position,
            targetPos,
            Time.deltaTime * followSpeed
        );

        heldItem.transform.rotation = Quaternion.Lerp(
            heldItem.transform.rotation,
            targetRot,
            Time.deltaTime * followSpeed
        );
    }
}