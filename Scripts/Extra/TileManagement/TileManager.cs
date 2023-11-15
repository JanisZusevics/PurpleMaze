using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject[] tilePrefabs;
    private Vector3 playerAverageLocation;
    private float tileSize;
    private List<GameObject> activeTiles;
    private float tileLength;
    private Vector3 currentTile;
    public int gridSize = 3;
    public GameObject spawnedParent;
    void Start()
    {
        activeTiles = new List<GameObject>();

        // Set tileSize to the size of the first tile prefab
        tileSize = tilePrefabs[0].GetComponent<Renderer>().bounds.size.x;

        tileLength = tileSize; // Set tileLength to tileSize

        // Calculate the average location of all gameObjects with the "Player" tag
        playerAverageLocation = GetPlayerAverageLocation();

        // Calculate the initial current tile based on the average location and the tile size
        currentTile = new Vector3(
            Mathf.Round(playerAverageLocation.x / tileSize),
            0,
            Mathf.Round(playerAverageLocation.z / tileSize)
        );

        // Spawn a 3x3 grid of tiles centered on the current tile
        SpawnGrid();
        RedrawGrid();
    }
    void Update()
    {
        playerAverageLocation = GetPlayerAverageLocation();

        Vector3 playerTile = new Vector3(
            Mathf.Round(playerAverageLocation.x / tileSize),
            0,
            Mathf.Round(playerAverageLocation.z / tileSize)
        );

        if (playerTile != currentTile)
        {
            currentTile = playerTile;
            RedrawGrid();
        }

        // Calculate the outer bounds of the grid
        Vector3 gridMin = currentTile * tileSize - new Vector3(tileSize, 0, tileSize);
        Vector3 gridMax = currentTile * tileSize + new Vector3(tileSize, 0, tileSize);

        
        //Debug.Log("Player average location: " + playerAverageLocation + ", Grid bounds: Min = " + gridMin + ", Max = " + gridMax);
        Debug.DrawLine(playerAverageLocation, playerAverageLocation + Vector3.up * 10, Color.red);
        Debug.DrawLine(gridMin, gridMin + Vector3.up * 10, Color.green);
        Debug.DrawLine(gridMax, gridMax + Vector3.up * 10, Color.blue);

    }

    void SpawnGrid()
    {
        int halfSize = gridSize / 2;
        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int z = -halfSize; z <= halfSize; z++)
            {
                Vector3 tilePosition = new Vector3(
                    (currentTile.x + x) * tileSize,
                    0,
                    (currentTile.z + z) * tileSize
                );
                SpawnTile((int)tilePosition.x, (int)tilePosition.z);
            }
        }
    }
    void RedrawGrid()
    {
        int halfSize = gridSize / 2;
        for (int i = activeTiles.Count - 1; i >= 0; i--)
        {
            Vector3 tilePos = activeTiles[i].transform.position;
            if (Mathf.Abs(tilePos.x - currentTile.x) > halfSize || Mathf.Abs(tilePos.z - currentTile.z) > halfSize)
            {
                foreach (Transform child in activeTiles[i].transform)
                {
                     child.SetParent(spawnedParent.transform);
                 }
                // Then destroy the tile itself
                Destroy(activeTiles[i]);
                activeTiles.RemoveAt(i);
            }
        }


        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int z = -halfSize; z <= halfSize; z++)
            {
                Vector3 tilePos = new Vector3(currentTile.x + x, 0, currentTile.z + z);
                if (!TileExists(tilePos))
                {
                    SpawnTile((int)tilePos.x, (int)tilePos.z);
                }
            }
        }
    }

    Vector3 GetPlayerAverageLocation()
    {
        Vector3 averageLocation = gameManager.playerAverageLocation;
        //Debug.Log("Average player location: " + averageLocation);
        return averageLocation;
    }

    bool TileExists(Vector3 position)
    {
        foreach (GameObject tile in activeTiles)
        {
            if (tile.transform.position == position)
            {
                return true;
            }
        }
        return false;
    }
    void SpawnTile(int gridX, int gridZ, int prefabIndex = -1)
    {
        GameObject go;
        prefabIndex = RandomPrefabIndex();
        go = Instantiate(tilePrefabs[prefabIndex]) as GameObject;
        go.transform.SetParent(transform);

        Vector3 spawnPosition = new Vector3(gridX, 0, gridZ) * tileSize;
        go.transform.position = spawnPosition;

        // Round the tile position to the nearest multiple of the tile size
        go.transform.position = new Vector3(
            Mathf.Round(go.transform.position.x / tileSize) * tileSize,
            0,
            Mathf.Round(go.transform.position.z / tileSize) * tileSize
        );

        //Debug.Log("Spawned tile at: " + go.transform.position);

        activeTiles.Add(go);
    }

    void DeleteTile()
    {
        //Debug.Log("Deleting tile at: " + activeTiles[0].transform.position);
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

    int RandomPrefabIndex()
    {
        if (tilePrefabs.Length <= 1)
            return 0;

        int randomIndex = Random.Range(0, tilePrefabs.Length);
        //Debug.Log("Selected prefab index: " + randomIndex);
        return randomIndex;
    }
}