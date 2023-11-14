using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 offset;          // Offset from the players
    public float smoothSpeed = 0.5f; // Smoothing speed

    void LateUpdate()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Vector3 averagePosition = GetAveragePosition(players) + offset;

        // Smoothly transition to the new position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, averagePosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    Vector3 GetAveragePosition(GameObject[] players)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (GameObject player in players)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (player != null && playerMovement != null && playerMovement.isActive)
            {
                sum += player.transform.position;
                count++;
            }
        }

        return count > 0 ? sum / count : Vector3.zero;
    }
}