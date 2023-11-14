/// <summary>
/// Destroys game objects that fall below a certain height threshold.
/// </summary>
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    public float heightThreshold = -10f; // The height below which objects will be destroyed

    
    void Update()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ; // Find all game objects
        foreach(GameObject go in allObjects) // Iterate through them
        {
            Rigidbody rb = go.GetComponent<Rigidbody>(); // Get the rigidbody
            if (rb != null && go.transform.position.y < heightThreshold) // If the rigidbody exists and the object is below the threshold
            {
                Destroy(go); // Destroy the object
            }
        }
    }
}