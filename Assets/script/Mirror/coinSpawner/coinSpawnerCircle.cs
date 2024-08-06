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
            float angle = i * Mathf.PI * 2 / numItemsToSpawn;
            if (angle > (maxAngle * Mathf.PI / 180))
            {
                break;
            }
            SpreadItem(angle + (offsetAngle * Mathf.PI / 180));
        }
    }
    [Server]
    void SpreadItem(float angle)
    {
        Vector3 randPosition = new Vector3(Mathf.Cos(angle) * radius, Random.Range(-itemYSpread, itemYSpread), Mathf.Sin(angle) * radius) + transform.position;
        GameObject clone = Instantiate(itemToSpread, randPosition, itemToSpread.transform.rotation);
        NetworkServer.Spawn(clone);
    }
}
