using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class lineCoinRaycastSpawner : NetworkBehaviour
{
    public GameObject itemToSpread;
    public Transform[] coinLocation;
    public float itemXSpread = 30f;
    public float itemYSpread = 0;
    public float itemZSpread = 8f;

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        base.OnStartServer();
        for (int i = 0; i < coinLocation.Length; i++)
        {
            SpreadItem(coinLocation[i]);
        }
    }
    [Server]
    void SpreadItem(Transform location)
    {
        GameObject clone = Instantiate(itemToSpread, location.position, itemToSpread.transform.rotation);
        NetworkServer.Spawn(clone);
    }
}
