using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    public float distanceThreshold = 10f; // Threshold for object destruction
    public float shadowDistanceThreshold = 5f; // Half the distance threshold for shadow management
    public List<string> tagsToCheck; // Tags to check for object management
    public GameObject playerMover; // Empty object that player objects move towards
    public float checkInterval = 0.5f; // Interval for checking object distance
    public float shadowCheckInterval = 0.5f; // Interval for checking shadow distance

    void Start()
    {
        StartCoroutine(CheckDistance());
        StartCoroutine(ManageShadows());
    }

    IEnumerator CheckDistance()
    {
        while (true)
        {
            foreach (string tag in tagsToCheck)
            {
                GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject go in objectsWithTag)
                {
                    float distance = Vector3.Distance(go.transform.position, playerMover.transform.position);
                    if (distance > distanceThreshold)
                    {
                        Destroy(go);
                    }
                }
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    IEnumerator ManageShadows()
    {
        while (true)
        {
            foreach (string tag in tagsToCheck)
            {
                GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject go in objectsWithTag)
                {
                    float distance = Vector3.Distance(go.transform.position, playerMover.transform.position);
                    Renderer renderer = go.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.shadowCastingMode = (distance <= shadowDistanceThreshold) ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                }
            }
            yield return new WaitForSeconds(shadowCheckInterval);
        }
    }
}
