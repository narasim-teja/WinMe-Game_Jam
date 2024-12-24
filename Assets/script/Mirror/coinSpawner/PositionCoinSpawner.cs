using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PositionCoinSpawner : NetworkBehaviour
{
    [SerializeField] Transform spawnerParent;
    [SerializeField] float spacing;
    [SerializeField] int numberOfColumns;
    public override void OnStartServer()
    {
        base.OnStartServer();
        for (int i = 0; i < spawnerParent.childCount; i++)
        {
            SpreadItem(spawnerParent.GetChild(i).position);
            for (int j = 1; j <= numberOfColumns; j++)
            {
                SpreadItem(spawnerParent.GetChild(i).position - new Vector3(0, 0, j * spacing));
                SpreadItem(spawnerParent.GetChild(i).position + new Vector3(0, 0, j * spacing));
            }
        }


    }

    [Server]
    void SpreadItem(Vector3 location)
    {
        Spawner.SpawnCoin(location);
        //GameObject clone = Instantiate(itemToSpread, location, itemToSpread.transform.rotation);
        //NetworkServer.Spawn(clone);
    }
}
