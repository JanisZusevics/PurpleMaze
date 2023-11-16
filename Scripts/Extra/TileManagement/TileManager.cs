using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject[] tilePrefabs;
    private List<GameObject> activeTiles = new List<GameObject>();
    private float tileSize;
    private Vector3 lastTilePosition;
    public int gridSize = 3;

    void Start()
    {
        tileSize = tilePrefabs[0].GetComponent<Renderer>().bounds.size.x;
        lastTilePosition = CalculateTilePosition(gameManager.playerAverageLocation);
        SpawnGrid(lastTilePosition);
    }

    void Update()
    {
        Vector3 currentTilePosition = CalculateTilePosition(gameManager.playerAverageLocation);
        if (currentTilePosition != lastTilePosition)
        {
            UpdateGrid(currentTilePosition);
            lastTilePosition = currentTilePosition;
        }
    }

    private void SpawnGrid(Vector3 centerTile)
    {
        int halfSize = gridSize / 2;
        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int z = -halfSize; z <= halfSize; z++)
            {
                Vector3 tilePosition = CalculateTilePosition(centerTile, x, z);
                if (!TileExists(tilePosition))
                {
                    InstantiateTile(tilePosition);
                }
            }
        }
    }

    private void UpdateGrid(Vector3 newCenterTile)
    {
        // Optionally add logic to remove tiles that are now outside the grid
        SpawnGrid(newCenterTile);
    }

    private void InstantiateTile(Vector3 position)
    {
        GameObject tilePrefab = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
        GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
        activeTiles.Add(tile);
    }

    private Vector3 CalculateTilePosition(Vector3 position, int offsetX = 0, int offsetZ = 0)
    {
        return new Vector3(
            Mathf.Floor(position.x / tileSize) + offsetX,
            0,
            Mathf.Floor(position.z / tileSize) + offsetZ
        ) * tileSize;
    }

    private bool TileExists(Vector3 position)
    {
        foreach (GameObject tile in activeTiles)
        {
            if (tile.transform.position == position)
                return true;
        }
        return false;
    }
}
