using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private Vector3 last;

    private void Start() => last = transform.position;

    private void Update()
    {
        Vector3 pos = transform.position;
        Vector3 delta = pos - last;
        delta.y = 0;
        last = pos;

        float speed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        animator.SetFloat(SpeedHash, speed, 0.1f, Time.deltaTime);
    }
}