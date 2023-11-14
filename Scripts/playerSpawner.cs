using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // The player prefab to spawn
    public GameObject tile; // The tile to spawn the player on
    public float spawnInterval = 5f; // The interval between spawns

    private Vector3 tileSize; // The size of the tile

    void Start()
    {
        tileSize = tile.GetComponent<Renderer>().bounds.size;
        StartCoroutine(SpawnPlayer());
    }

    IEnumerator SpawnPlayer()
    {
        while (true)
        {
            float x = NextGaussian(0, tileSize.x / 4);
            float z = NextGaussian(0, tileSize.z / 4);

            Vector3 spawnPosition = new Vector3(x, 0, z);

            Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

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