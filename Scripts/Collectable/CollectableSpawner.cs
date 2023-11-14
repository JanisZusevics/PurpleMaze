using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectableSpawner : MonoBehaviour
{
    public static CollectableSpawner instance; // Singleton instance

    public GameObject collectablePrefab;
    public GameObject plane;
    public GameObject playerPrefab;            // Player prefab
    public float spawnHeight = 1.0f;           // Height at which the collectables spawn
    public int poolSize = 10;                  // Number of collectables to pool
    public float spawnInterval = 2.0f;

    private float timer;
    private Vector3 planeSize;
    private List<GameObject> collectablesPool;
    private int collectablesCollected = 0;     // Count of collected collectables
    public TMP_Text collectablesText;

    private void Awake()
    {
        // Setup the singleton instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (plane != null)
        {
            MeshRenderer planeMeshRenderer = plane.GetComponent<MeshRenderer>();
            planeSize = planeMeshRenderer.bounds.size;
        }
        else
        {
            Debug.LogError("Plane GameObject not assigned!");
        }

        // Initialize the pool
        collectablesPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(collectablePrefab, new Vector3(0, -spawnHeight, 0), Quaternion.identity);
            obj.SetActive(false);
            collectablesPool.Add(obj);
        }

        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && playerPrefab != null)
        {
            SpawnCollectable();
            timer = spawnInterval;
        }
    }

    void SpawnCollectable()
    {
        foreach (GameObject collectable in collectablesPool)
        {
            if (!collectable.activeInHierarchy)
            {
                float spawnPosX = NextGaussian(plane.transform.position.x, planeSize.x / 4);
                float spawnPosZ = NextGaussian(plane.transform.position.z, planeSize.z / 4);
                Vector3 spawnPosition = new Vector3(spawnPosX, spawnHeight, spawnPosZ);

                collectable.transform.position = spawnPosition;
                collectable.SetActive(true);
                break; // Only spawn one at a time
            }
        }
    }

    // Generate a random number with a Gaussian distribution
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

    // Function to increment the collected count and log it
    public void CollectableCollected()
    {
        collectablesCollected++;
        collectablesText.text = "Orbs Collected: " + collectablesCollected;
        Debug.Log("Collectables Collected: " + collectablesCollected);
    }
}
