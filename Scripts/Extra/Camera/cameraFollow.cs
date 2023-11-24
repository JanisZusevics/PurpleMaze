using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject playerMover; // Reference to the playerMover object
    public Vector3 offset;         // Offset from the players
    // smoothing speed
    public float smoothSpeed = 0.5f; // Smoothing speed
    public bool isZoomEnabled = true; // Whether zooming is enabled

    void LateUpdate()
    {
        // Get the playerMover object's position
        Vector3 playerMoverPosition = playerMover.transform.position;

        // Calculate the desired position, ignoring the y-component of the player's position
        Vector3 desiredPosition = new Vector3(playerMoverPosition.x, transform.position.y, playerMoverPosition.z) + offset;

        // Smoothly transition to the new position, ignoring the y-component
        Vector3 smoothedPosition = Vector3.Lerp(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(desiredPosition.x, 0, desiredPosition.z), smoothSpeed * Time.deltaTime);
        transform.position = new Vector3(smoothedPosition.x, transform.position.y, smoothedPosition.z);

        // If zooming is enabled, adjust the y-component of the camera's position based on the scroll wheel value
        if (isZoomEnabled)
        {
            // Get the scroll wheel value
            float scrollValue = Input.GetAxis("Mouse ScrollWheel");

            // If the scroll wheel is being used
            if (Mathf.Abs(scrollValue) > 0.01f)
            {
                // Define a zoom speed
                float zoomSpeed = 10.0f;

                // Adjust the y-component of the camera's position based on the scroll wheel value
                transform.position = new Vector3(transform.position.x, transform.position.y - scrollValue * zoomSpeed, transform.position.z);
            }
        }
    }
}