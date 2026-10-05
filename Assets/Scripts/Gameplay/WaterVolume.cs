using UnityEngine;

public class WaterVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var move = other.GetComponentInParent<PlayerMovementController>();
        if (move != null) 
            move.SetInWater(true);
    }

    private void OnTriggerExit(Collider other)
    {
        var move = other.GetComponentInParent<PlayerMovementController>();
        if (move != null) 
            move.SetInWater(false);
    }
}