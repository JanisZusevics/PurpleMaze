using System.Collections.Generic;
using UnityEngine;

public class EndlessTileManager : MonoBehaviour
{
    public GameManager GameManager; // Empty object that player objects move towards
    [SerializeField] private GameObject[] tilePrefabs;
    private HashSet<Vector3> activeTilePositions = new HashSet<Vector3>();
    private Queue<GameObject> tilePool = new Queue<GameObject>();
    private Vector3 lastTilePosition;
    private float tileSize;
    public int gridSize = 3;


     private void Awake() {
        GameManager = GameObject.FindObjectOfType<GameManager>();
    }
    void Start()
    {        // Calculate the actual size of the tile's mesh
        Renderer renderer = tilePrefabs[0].GetComponent<Renderer>();
        tileSize = (renderer.bounds.size.x) * 0.83f;

        lastTilePosition = CalculateTilePosition(GameManager.King.transform.position);
        CreateInitialGrid();
    }

    void Update()
    {
        Vector3 currentTilePosition = CalculateTilePosition(GameManager.King.transform.position);
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
        const float epsilon = 0.1f; // Small value to account for floating point precision errors

        foreach (Transform child in transform)
        {
            if (Vector3.Distance(child.position, position) < epsilon)
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
            Mathf.Round(position.x / tileSize) * tileSize + offsetX * tileSize,
            0,
            Mathf.Round(position.z / tileSize) * tileSize + offsetZ * tileSize
        );
    }
}
