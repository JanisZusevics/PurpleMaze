using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject playerMover; // Reference to the playerMover object
    public Vector3 offset;         // Offset from the players
    public float smoothSpeed = 0.5f; // Smoothing speed

    void LateUpdate()
    {
        // Get the playerMover object's position
        Vector3 playerMoverPosition = playerMover.transform.position;

        Vector3 desiredPosition = playerMoverPosition + offset;

        // Smoothly transition to the new position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}