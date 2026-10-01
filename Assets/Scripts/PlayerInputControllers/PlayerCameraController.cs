using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera camera;

    [Header("Look")]
    [SerializeField] private float sensitivity = 0.12f;
    [SerializeField] private float minPitch = -89f;
    [SerializeField] private float maxPitch = 89f;

    [Header("Input")]
    [SerializeField] private InputActionReference lookAction;

    private float pitch;

    public override void OnStartClient()
    {
        if (!IsOwner)
        {
            if (camera != null) camera.enabled = false;
            return;
        }

        lookAction.action.Enable();
    }

    public override void OnStopClient()
    {
        if (!IsOwner) return;

        lookAction.action.Disable();
    }

    private void LateUpdate()
    {
        if (!IsOwner) return;

        var look = lookAction.action.ReadValue<Vector2>();

        cameraRoot.Rotate(Vector3.up, look.x * sensitivity, Space.Self);

         pitch -= look.y * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}