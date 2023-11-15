using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public string parentObjectTag = "spawnerFolder"; // The parent GameObject for the spawned objects
    private GameObject parentObject; // The parent GameObject for the spawned objects
    public float spawnHeight = 1.0f;

    private Vector3 tileSize; // Add this line

    void Start()
    {
        tileSize = GetComponent<Renderer>().bounds.size;

        // Find the parent object by its name and tag
        parentObject = GameObject.FindWithTag(parentObjectTag);

        Spawn();
    }

    void Spawn()
    {
        float spawnPosX = NextGaussian(0, tileSize.x / 4);
        float spawnPosZ = NextGaussian(0, tileSize.z / 4);

        Vector3 spawnPosition = new Vector3(spawnPosX, spawnHeight, spawnPosZ);

        // Instantiate the object as a child of parentObject
        GameObject spawnedObject = Instantiate(prefab, transform.position + spawnPosition, Quaternion.identity, parentObject.transform);
    }

    private float NextGaussian(float mean, float standardDeviation)
    {
        float v1, v2, s;
        do
        {
            v1 = 2.0f * Random.Range(0f,1f) - 1.0f;
            v2 = 2.0f * Random.Range(0f,1f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        }
        while (s >= 1.0f || s == 0f);

        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

        return mean + standardDeviation * v1 * s;
    }
}