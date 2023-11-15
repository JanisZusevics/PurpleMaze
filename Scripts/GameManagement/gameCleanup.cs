using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    public float distanceThreshold = 50f; // The distance from the average player location beyond which objects will be destroyed
    public List<GameObject> objectsToCheck; // The list of objects to check
    public GameManager gameManager; // Reference to the GameManager script

        void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
        void Update()
    {
        Vector3 averagePlayerLocation = gameManager.playerAverageLocation;

        // Create a list to hold objects that need to be removed
        List<GameObject> objectsToRemove = new List<GameObject>();

        for (int i = 0; i < objectsToCheck.Count; i++)
        {
            GameObject go = objectsToCheck[i];
            if (go == null)
            {
                objectsToRemove.Add(go);
                continue;
            }

            float distance = Vector3.Distance(go.transform.position, averagePlayerLocation);
            if (distance > distanceThreshold)
            {
                // Destroy all children of the game object
                foreach (Transform child in go.transform)
                {
                    Destroy(child.gameObject);
                }

                objectsToRemove.Add(go);
            }
        }

        // Remove objects from objectsToCheck
        foreach (GameObject go in objectsToRemove)
        {
            objectsToCheck.Remove(go);
        }
    }
}