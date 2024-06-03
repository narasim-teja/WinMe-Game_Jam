using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnCoins : MonoBehaviour
{
    public GameObject[] itemsToPickFrom;
    public float raycastDistance = 100f;
    public float overlapTestBoxSize = 1f;
    public LayerMask spawnedObjectLayer;
    public float offsetFromGround = 0.7f;

    // Start is called before the first frame update
    void Start()
    {
        PositionRaycast();
    }

    void PositionRaycast()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance))
        {
            int floorLayerIndex = LayerMask.NameToLayer("ground");

            if ( hit.transform.gameObject.layer != floorLayerIndex) return;

            Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            Vector3 overlapTestBoxScale = new Vector3(overlapTestBoxSize, overlapTestBoxSize, overlapTestBoxSize);
            Collider[] collidersInsideOverlapBox = new Collider[1];
            int numberOfCollidersFound = Physics.OverlapBoxNonAlloc(hit.point, overlapTestBoxScale, collidersInsideOverlapBox, spawnRotation, spawnedObjectLayer);

            Debug.Log("number of colliders found " + numberOfCollidersFound);

            if (numberOfCollidersFound == 0)
            {
                Debug.Log("spawned robot");
                Pick(hit.point, spawnRotation);
            }
            else
            { 
                Debug.Log("name of collider 0 found " + collidersInsideOverlapBox[0].name);
            }
        }
    }

    void Pick(Vector3 positionToSpawn, Quaternion rotationToSpawn)
    {
        int randomIndex = Random.Range(0, itemsToPickFrom.Length);
        GameObject clone = Instantiate(itemsToPickFrom[randomIndex], positionToSpawn + new Vector3(0, offsetFromGround, 0), rotationToSpawn);
        GameObject clone1 = Instantiate(itemsToPickFrom[randomIndex], positionToSpawn + new Vector3(1, offsetFromGround, 0), rotationToSpawn);
        GameObject clone2 = Instantiate(itemsToPickFrom[randomIndex], positionToSpawn + new Vector3(1, offsetFromGround, 1), rotationToSpawn);
        GameObject clone3 = Instantiate(itemsToPickFrom[randomIndex], positionToSpawn + new Vector3(1, offsetFromGround, -1), rotationToSpawn);
        GameObject clone4 = Instantiate(itemsToPickFrom[randomIndex], positionToSpawn + new Vector3(2, offsetFromGround, 0), rotationToSpawn);
    }
}
