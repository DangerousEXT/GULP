using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ShittyMovementTest : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float gravity = -20f;

    private CharacterController cc;
    private float verticalVelocity;

    private void Awake() => cc = GetComponent<CharacterController>();

    public override void OnStartClient()
    {
        // у чужих позицию ставит NetworkTransform, контроллер ему мешает
        cc.enabled = IsOwner;
    }

    private void Update()
    {
        if (!IsOwner) return;

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        input = Vector3.ClampMagnitude(input, 1f);

        verticalVelocity = cc.isGrounded ? -2f : verticalVelocity + gravity * Time.deltaTime;

        Vector3 move = input * speed;
        move.y = verticalVelocity;
        cc.Move(move * Time.deltaTime);

        if (input.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(input);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }
    }
}