using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    public float heightThreshold = -10f; // The height below which objects will be destroyed

    void Update()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
        foreach(GameObject go in allObjects)
        {
            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb != null && go.transform.position.y < heightThreshold)
            {
                Destroy(go);
            }
        }
    }
}