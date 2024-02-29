using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    [Header("Coordinates")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float minZ;
    public float maxZ;

    [Header("Spawn amounts")]
    public int numberOfCoins;
    public int numberOfBatteries;
    public int numberOfRockets;
    public int numberOfDaddy;
    public int numberOfMommy;
    public int numberOfSlime;
    public int numberOfOil;

    [Header("Spawn objects")]
    public GameObject coins;
    public GameObject battery;
    public GameObject rocket;
    public GameObject daddyCactus;
    public GameObject mommyCactus;
    public GameObject puddleSlime;
    public GameObject puddleOil;

    private MeshCollider meshCollider;
    private GameObject newObject;

    void Start()
    {
        meshCollider = gameObject.GetComponentInParent<MeshCollider>();

        for (int i = 0; i < numberOfCoins; i++)
        {
            Spawn(coins);
        }

        for (int i = 0; i < numberOfBatteries; i++)
        {
            Spawn(battery);
        }

        for (int i = 0; i < numberOfRockets; i++)
        {
            Spawn(rocket);
        }

        for (int i = 0; i < numberOfDaddy; i++)
        {
            Spawn(daddyCactus);
        }

        for (int i = 0; i < numberOfMommy; i++)
        {
            Spawn(mommyCactus);
        }

        for (int i = 0; i < numberOfSlime; i++)
        {
            Spawn(puddleSlime);
            newObject.transform.Rotate(-90f, 0f, 0f);
        }

        for (int i = 0; i < numberOfOil; i++)
        {
            Spawn(puddleOil);
            newObject.transform.Rotate(-90f, 0f, 0f);
        }
    }

    /// <summary>
    /// Pick a random position based on a range of X, Y, and Z values.
    /// Find the point that is closest to the mesh collider of the parent object and spawn an object at that point.
    /// </summary>
    void Spawn(GameObject obj)
    {
        Vector3 randomSpawnPosition = new Vector3(Random.Range(minX, maxX),Random.Range(minY, maxY), Random.Range(minZ, maxZ));
        newObject = Instantiate(obj, meshCollider.ClosestPoint(randomSpawnPosition), Quaternion.identity);
    }
}
