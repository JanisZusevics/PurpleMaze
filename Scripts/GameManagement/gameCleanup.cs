using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    // The distance from the average player location beyond which objects will be destroyed
    public float distanceThreshold = 10f; 

    // The list of tags to check. Objects with these tags will be considered for destruction
    public List<string> tagsToCheck; 

    // Reference to the GameManager script. This is used to get the average player location
    public GameManager gameManager; 

    // The interval in seconds between checks. This determines how often we check the distance of objects
    public float checkInterval = 0.5f; 

    void Start()
    {
        // Find the GameManager in the scene and store a reference to it
        gameManager = FindObjectOfType<GameManager>();

        // Start the CheckDistance coroutine. This will run in the background and periodically check the distance of objects
        StartCoroutine(CheckDistance());
    }

    IEnumerator CheckDistance()
    {
        // This loop will run forever
        while (true)
        {
            // Get the average player location from the GameManager
            Vector3 averagePlayerLocation = gameManager.playerAverageLocation;

            // Loop over all the tags we need to check
            foreach (string tag in tagsToCheck)
            {
                // Find all the game objects with the current tag
                GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);

                // Loop over all the game objects with the current tag
                foreach (GameObject go in objectsWithTag)
                {
                    // Calculate the distance from the game object to the average player location
                    float distance = Vector3.Distance(go.transform.position, averagePlayerLocation);

                    // If the distance is greater than the threshold, destroy the game object
                    if (distance > distanceThreshold)
                    {
                        Destroy(go); 
                    }
                }
            }

            // Wait for the specified interval before the next check. This prevents the checks from running every frame, which could be performance intensive
            yield return new WaitForSeconds(checkInterval); 
        }
    }
}