using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] spawners;
    public Vector2 spawnRadiusRange = new Vector2(10f, 50f);
    public float baseSpawnInterval = 1f;
    public float minSpawnInterval = 0.1f;
    public float maxSpawnInterval = 2f;
    public float speedForMinInterval = 10f;
    public float biasStrength = 3f;

    private float[] spawnerHeights; // Array to store the heights of the spawners

    public void InitializeSpawnerHeights()
    {
        // Calculate and store the heights of the spawners
        spawnerHeights = new float[spawners.Length];
        for (int i = 0; i < spawners.Length; i++)
        {
            if (spawners[i].GetComponent<MeshRenderer>() != null)
                spawnerHeights[i] = spawners[i].GetComponent<MeshRenderer>().bounds.size.y;
            else if (spawners[i].GetComponent<Collider>() != null)
                spawnerHeights[i] = spawners[i].GetComponent<Collider>().bounds.size.y;
            else
                spawnerHeights[i] = 0;
        }
    }

    public IEnumerator SpawnRandomObjectInRange(GameObject playerMover)
    {
        while (true)
        {
            float playerSpeed = playerMover.GetComponent<PlayerMovement>().velocity.magnitude;
            float adjustedSpawnInterval = Mathf.Lerp(maxSpawnInterval, minSpawnInterval, playerSpeed / speedForMinInterval);

            yield return new WaitForSeconds(adjustedSpawnInterval);

            int spawnerIndex = Random.Range(1, spawners.Length);
            float rand = Mathf.Sqrt(-2.0f * Mathf.Log(Random.Range(0f, 1f))) * Mathf.Sin(2.0f * Mathf.PI * Random.Range(0f, 1f));
            float distance = spawnRadiusRange.y + rand * spawnRadiusRange.y / biasStrength;
            distance = Mathf.Clamp(distance, 0, spawnRadiusRange.y);
            float angle = Random.Range(0, 360);
            Vector3 spawnPosition = playerMover.transform.position + new Vector3(distance * Mathf.Cos(angle), 0, distance * Mathf.Sin(angle));
            spawnPosition.y += spawnerHeights[spawnerIndex] / 2;

            Instantiate(spawners[spawnerIndex], spawnPosition, Quaternion.identity, transform);
        }
    }
}