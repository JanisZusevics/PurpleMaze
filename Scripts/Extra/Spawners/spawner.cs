using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject crown;
    public GameObject[] objectsToSpawn;
    public Vector2 spawnRadiusRange = new Vector2(10f, 50f);
    // The frequency at which objects are spawned in seconds (min, max)
    public Vector2 spawnFrequency = new Vector2(1f, 5f);

    private float[] objectToSpawnHeights;

    // Start is called before the first frame update
    void Start()
    {
        crown = GameObject.Find("Crown");
        // log objectToSpawnHeights
        //Debug.Log("before "+ objectToSpawnHeights);
        CalculateSpawnerHeights();
        // Log new heights 
        //Debug.Log("After "+objectToSpawnHeights);
        StartCoroutine(SpawnRandomObjectInRange());
    }

    // Calculate the height of each object in the objectsToSpawn array
    private void CalculateSpawnerHeights()
    {
        objectToSpawnHeights = new float[objectsToSpawn.Length];

        for (int i = 0; i < objectsToSpawn.Length; i++)
        {
            // Get the collider of the object
            Collider objectCollider = objectsToSpawn[i].GetComponent<Collider>();
            // Get the height of the object
            float objectHeight = objectCollider.bounds.size.y;
            // Set the object's height in the array
            objectToSpawnHeights[i] = objectHeight;
        }
    }
    public IEnumerator SpawnRandomObjectInRange()
    {
        while (true)
        {
            // Get random number in range of the objectsToSpawn array
            int randomIndex = Random.Range(0, objectsToSpawn.Length);
            // get object to spawns height from the objectToSpawnHeights array
            float objectHeight = objectToSpawnHeights[randomIndex];
            // Get the spawn radius from the crown object
            float spawnRadius = Random.Range(spawnRadiusRange.x, spawnRadiusRange.y);
            // Get the spawn position from the crown object
            Vector3 spawnPosition = crown.transform.position;
            // Set the spawn position's x and z values to a random position within the spawn radius
            spawnPosition.x += Random.Range(-spawnRadius, spawnRadius);
            spawnPosition.z += Random.Range(-spawnRadius, spawnRadius);
            // Set the spawn position's y value to the object's height
            spawnPosition.y = objectHeight;
            // Spawn the object
            Instantiate(objectsToSpawn[randomIndex], spawnPosition, Quaternion.identity);
            // Wait for the spawn frequency
            yield return new WaitForSeconds(Random.Range(spawnFrequency.x, spawnFrequency.y));
        }
    }
}