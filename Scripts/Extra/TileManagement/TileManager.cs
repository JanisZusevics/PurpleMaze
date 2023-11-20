using System.Collections.Generic;
using UnityEngine;

public class EndlessTileManager : MonoBehaviour
{
    public GameObject playerMover; // Empty object that player objects move towards
    [SerializeField] private GameObject[] tilePrefabs;
    private HashSet<Vector3> activeTilePositions = new HashSet<Vector3>();
    private Queue<GameObject> tilePool = new Queue<GameObject>();
    private Vector3 lastTilePosition;
    private float tileSize;
    public int gridSize = 3;

    void Start()
    {
        // Calculate the actual size of the tile's mesh
        Mesh mesh = tilePrefabs[0].GetComponent<MeshFilter>().sharedMesh;
        tileSize = mesh.bounds.size.x * tilePrefabs[0].transform.localScale.x;

        lastTilePosition = CalculateTilePosition(playerMover.transform.position);
        CreateInitialGrid();
    }

    void Update()
    {
        Vector3 currentTilePosition = CalculateTilePosition(playerMover.transform.position);
        if (currentTilePosition != lastTilePosition)
        {
            UpdateGrid(currentTilePosition);
            lastTilePosition = currentTilePosition;
        }
    }

    private void CreateInitialGrid()
    {
        UpdateGrid(lastTilePosition);
    }

    private void UpdateGrid(Vector3 newCenterTile)
    {
        int halfSize = (gridSize - 1) / 2;
        HashSet<Vector3> newActiveTilePositions = new HashSet<Vector3>();

        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int z = -halfSize; z <= halfSize; z++)
            {
                Vector3 tilePosition = CalculateTilePosition(newCenterTile, x, z);
                newActiveTilePositions.Add(tilePosition);

                if (!activeTilePositions.Contains(tilePosition))
                {
                    InstantiateOrUpdateTile(tilePosition);
                }
            }
        }

        foreach (var oldPosition in activeTilePositions)
        {
            if (!newActiveTilePositions.Contains(oldPosition))
            {
                RecycleTileAtPosition(oldPosition);
            }
        }

        activeTilePositions = newActiveTilePositions;
    }


    private void InstantiateOrUpdateTile(Vector3 position)
    {
        GameObject tile;
        if (tilePool.Count > 0)
        {
            tile = tilePool.Dequeue();
            tile.transform.position = position;
            tile.SetActive(true);
        }
        else
        {
            GameObject tilePrefab = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
            tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
        }
    }

    private void RecycleTileAtPosition(Vector3 position)
    {
        foreach (Transform child in transform)
        {
            if (child.position == position)
            {
                child.gameObject.SetActive(false);
                tilePool.Enqueue(child.gameObject);
                break;
            }
        }
    }

    private Vector3 CalculateTilePosition(Vector3 position, int offsetX = 0, int offsetZ = 0)
    {
        return new Vector3(
            Mathf.Floor(position.x / tileSize) * tileSize + offsetX * tileSize,
            0,
            Mathf.Floor(position.z / tileSize) * tileSize + offsetZ * tileSize
        );
    }
}
