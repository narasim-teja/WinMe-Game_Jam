using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CoinScript : NetworkBehaviour
{
    public float degreesPerSecond = 15f;
    public float amplitude = 0.25f; // how high the item floats
    public float frequency = 1f;

    public bool isPicked = false;
    // position storage variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    void Start()
    {
        //store starting orientation
        posOffset = transform.position;
    }

    void Update()
    {
        transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

        // float with Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            RewardPoint(other.gameObject);
    }

    [ServerCallback]
    void RewardPoint(GameObject player)
    {
        if (!isPicked)
        {
            isPicked = true;
            player.GetComponent<CarUIManager>().coinCount += 1;
            RpcDisableCoin(player);
            Invoke(nameof(RespawnCoin), 5f);
        }
    }

    void RespawnCoin()
    {
        Spawner.SpawnCoin(transform.position);
        // We can re-enable instead of respawn
        NetworkServer.Destroy(gameObject);
    }

    [ClientRpc]
    void RpcDisableCoin(GameObject player)
    {
        player.transform.Find("Audio").GetComponent<CarAudio>().PlayCoinPickUp();
        gameObject.SetActive(false);
    }
}
