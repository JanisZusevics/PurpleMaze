using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    public float distanceThreshold = 10f; // The distance from the average player location beyond which objects will be destroyed
    public List<string> tagsToCheck; // The list of tags to check
    public GameManager gameManager; // Reference to the GameManager script
    public float checkInterval = 0.5f; // The interval in seconds between checks

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        StartCoroutine(CheckDistance());
    }

    IEnumerator CheckDistance()
    {
        while (true)
        {
            Vector3 averagePlayerLocation = gameManager.playerAverageLocation;

            foreach (string tag in tagsToCheck)
            {
                GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);

                foreach (GameObject go in objectsWithTag)
                {
                    float distance = Vector3.Distance(go.transform.position, averagePlayerLocation);
                    if (distance > distanceThreshold)
                    {
                        Destroy(go); // Destroy the game object
                    }
                }
            }

            yield return new WaitForSeconds(checkInterval); // Wait for the specified interval before the next check
        }
    }
}