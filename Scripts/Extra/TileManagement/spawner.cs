using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public string parentObjectTag = "spawnerFolder";
    private Transform parentTransform; // Use Transform directly
    public float spawnHeight = 1.0f;
    private Vector3 tileSize;

    void Start()
    {
        parentTransform = GameObject.FindWithTag(parentObjectTag).transform; // Get transform directly
        tileSize = GetComponent<Renderer>().bounds.size;
        Spawn(tileSize); // Spawn on start
    }

        void OnEnable()
    {
        Spawn(tileSize); // Spawn when enabled
    }

    void Spawn(Vector3 tileSize)
    {
        Vector3 spawnPosition = new Vector3(NextGaussian(0, tileSize.x / 4), spawnHeight, NextGaussian(0, tileSize.z / 4));
        Instantiate(prefab, transform.position + spawnPosition, Quaternion.identity, parentTransform);
    }

    private float NextGaussian(float mean, float standardDeviation)
    {
        float v1, v2, s;
        do
        {
            v1 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            v2 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        }
        while (s >= 1.0f || s == 0f);

        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);
        return mean + standardDeviation * v1 * s;
    }
}