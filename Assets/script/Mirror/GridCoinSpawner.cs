using Mirror;
using UnityEngine;

public class GridCoinSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject prefab; 
    [SerializeField] private Vector2 gridSpacing = new Vector2(2.0f, 2.0f); 

    public override void OnStartServer()
    {
        base.OnStartServer();
        SpawnGrid();
    }

    [Server]
    private void SpawnGrid()
    {
        Vector3 startPosition = transform.position;
        int rows = 3;
        int columns = 3;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector3 spawnPosition = startPosition + new Vector3(column * gridSpacing.x, 0, row * gridSpacing.y);
                GameObject spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
                NetworkServer.Spawn(spawnedObject);
            }
        }
    }
}
