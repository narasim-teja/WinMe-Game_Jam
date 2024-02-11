using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float minZ;
    public float maxZ;
    public int numberOfCoins;
    public GameObject coins;

    private MeshCollider meshCollider;

    void Start()
    {
        meshCollider = gameObject.GetComponentInParent<MeshCollider>();

        for (int i = 0; i < numberOfCoins; i++)
        {
            Spawn();
        }
    }

    /// <summary>
    /// Pick a random position based on a range of X, Y, and Z values.
    /// Find the point that is closest to the mesh collider of the parent object and spawn a coin at that point.
    /// </summary>
    void Spawn()
    {
        Vector3 randomSpawnPosition = new Vector3(Random.Range(minX, maxX),Random.Range(minY, maxY), Random.Range(minZ, maxZ));

        GameObject coin = Instantiate(coins, meshCollider.ClosestPoint(randomSpawnPosition), Quaternion.identity);
        //Debug.Log($"Spawned a coin! x: {coin.transform.position.x} y: {coin.transform.position.y} z: {coin.transform.position.z}");
    }
}
