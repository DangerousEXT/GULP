using FishNet.Object;
using UnityEngine;

public class ShittyMovementTest : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;
    private void Update()
    {
        if (!IsOwner) return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        transform.position += new Vector3(x, 0, z) * speed * Time.deltaTime;
    }
}