using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // The player prefab to spawn
    public float spawnInterval = 5f; // The interval between spawns

    private Vector3 tileSize; // The size of the tile

    // Start is called before the first frame update
    void Start()
    {
        tileSize = GetComponent<Renderer>().bounds.size; // Get the size of the tile
        StartCoroutine(SpawnPlayer()); // Start spawning players
    }

    // Spawn a player at a random position on the tile
    IEnumerator SpawnPlayer()
    {
        while (true)
        {
            float x = NextGaussian(0, tileSize.x / 4);
            float z = NextGaussian(0, tileSize.z / 4);

            Vector3 spawnPosition = new Vector3(x, 1.0f, z);

            Instantiate(playerPrefab, transform.position + spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Generate a random number with a Gaussian distribution
    // The mean determines the most likely value (the center of the distribution)
    // The standard deviation determines the range of likely values (the width of the distribution)
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