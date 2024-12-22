using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GridCoinSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject itemToSpread;
    [SerializeField] private int xcount;
    [SerializeField] private int zcount;
    [SerializeField] private int xspacing;
    [SerializeField] private int zspacing;

    public override void OnStartServer()
    {
        base.OnStartServer();
        for(int i = 0; i < xcount; i++)
        {
            for (int j = 0; j < zcount; j++)
            {
                SpreadItem(i, j);
            }
        }
    }

    [Server]
    void SpreadItem(int i, int j)
    {
        Vector3 basePos = new(xspacing * i, 0, zspacing * j);
        Vector3 rotatedPos = transform.rotation * basePos + transform.position; // Rotate relative to original position

        Spawner.SpawnCoin(rotatedPos);
        //GameObject clone = Instantiate(itemToSpread, rotatedPos, Quaternion.identity);
        //NetworkServer.Spawn(clone);
    }
}
