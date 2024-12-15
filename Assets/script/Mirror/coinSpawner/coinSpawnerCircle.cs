using UnityEngine;
using Mirror;

public class coinSpawnerCircle : NetworkBehaviour
{
    public GameObject itemToSpread;
    public int numItemsToSpawn = 10;
    public float itemYSpread = 0;

    public float radius = 20f;

    public float maxAngle = 360;

    public float offsetAngle = 0;

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        base.OnStartServer();
        for (int i = 0; i < numItemsToSpawn; i++)
        {
            float angle = i * maxAngle / numItemsToSpawn;
            SpreadItem((angle + transform.eulerAngles.y) * Mathf.Deg2Rad);
        }
    }

    [Server]
    void SpreadItem(float angle)
    {
        Vector3 randPosition = new Vector3(Mathf.Sin(angle) * radius, Random.Range(-itemYSpread, itemYSpread), Mathf.Cos(angle) * radius) + transform.position;
        GameObject clone = Instantiate(itemToSpread, randPosition, itemToSpread.transform.rotation);
        NetworkServer.Spawn(clone);
    }
}
