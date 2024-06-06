using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class coinRaycastSpawner : NetworkBehaviour
{
    public GameObject itemToSpread;
    public int numItemsToSpawn = 10;

    public float itemXSpread = 30f;
    public float itemYSpread = 0;
    public float itemZSpread = 8f;

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        base.OnStartServer();
        for (int i = 0; i < numItemsToSpawn; i++)
        {
            SpreadItem();
        }
    }
    [Server]
    void SpreadItem()
    {
        Vector3 randPosition = new Vector3(Random.Range(-itemXSpread, itemXSpread), Random.Range(-itemYSpread, itemYSpread), Random.Range(-itemZSpread, itemZSpread)) + transform.position;
        GameObject clone = Instantiate(itemToSpread, randPosition, itemToSpread.transform.rotation);
        NetworkServer.Spawn(clone);
    }
}
