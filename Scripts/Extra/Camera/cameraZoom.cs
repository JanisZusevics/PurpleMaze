using UnityEngine;
using System.Collections;


public class CameraZoom : MonoBehaviour
{
    private GameObject player;

    // Function to shake the camera
    public void Shake()
    {
        player = getKing();
    }

    private GameObject getKing()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Mouse");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerMovement>().isKing)
            {
                return player;
            }
        }
        return null;
    }
    private IEnumerator ShakeCamera()
    {
    }
}