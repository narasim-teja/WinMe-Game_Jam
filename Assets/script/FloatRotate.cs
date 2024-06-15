using System.Collections;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public float degreesPerSecond = 15f;
    public float amplitude = 0.25f; // how high the item floats
    public float frequency = 1f;
    public bool isPicked = false;

    // position storage variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    public float minWaitTime = 3f;
    public float maxWaitTime = 10f;

    private Coroutine enableDisableCoroutine;

    void Start()
    {
        // Store starting orientation
        posOffset = transform.position;
    }

    void Update()
    {
        // Rotate the coin
        transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

        // Float with Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;

    }

}
