using FishNet.Example.ColliderRollbacks;
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

    private float yaw;
    private float pitch;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
        {
            camera.gameObject.SetActive(false);
            return;
        }

        yaw = transform.eulerAngles.y;
        lookAction.action.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (!IsOwner) return;

        var look = lookAction.action.ReadValue<Vector2>();
        yaw += look.x * sensitivity;
        pitch = Mathf.Clamp(pitch - look.y * sensitivity, minPitch, maxPitch);

        cameraRoot.rotation = Quaternion.Euler(0f, yaw, 0f); 
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    public override void OnStopClient()
    {
        if (!IsOwner) return;
        lookAction.action.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}