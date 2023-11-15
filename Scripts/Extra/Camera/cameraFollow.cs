using UnityEngine;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    public Vector3 offset;          // Offset from the players
    public float smoothSpeed = 0.5f; // Smoothing speed

    void LateUpdate()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> activePlayers = new List<GameObject>();

        foreach (GameObject player in allPlayers)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null && playerMovement.isActive)
            {
                activePlayers.Add(player);
            }
        }

        Vector3 averagePosition = GetAveragePosition(activePlayers.ToArray()) + offset;

        // Calculate the desired FOV
        float desiredFOV = 60 + activePlayers.Count * 2;

        // Clamp the FOV between 60 and 150
        float clampedFOV = Mathf.Clamp(desiredFOV, 60, 150);

        // Adjust the camera's FOV based on the number of active players
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, clampedFOV, Time.deltaTime);

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