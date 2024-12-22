using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class Spawner
{
    static float raycastDistance = 100f;
    static float overlapTestBoxSize = 1f;
    static LayerMask spawnedObjectLayer;
    static float offsetFromGround = 0.7f;
    private static (Vector3, Quaternion) PositionRaycast(Vector3 position)
    {

        if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, raycastDistance))
        {
            int floorLayerIndex = LayerMask.NameToLayer("ground");

            if (hit.transform.gameObject.layer != floorLayerIndex) return (Vector3.positiveInfinity, Quaternion.identity);

            Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            Vector3 overlapTestBoxScale = new Vector3(overlapTestBoxSize, overlapTestBoxSize, overlapTestBoxSize);
            Collider[] collidersInsideOverlapBox = new Collider[1];
            int numberOfCollidersFound = Physics.OverlapBoxNonAlloc(hit.point, overlapTestBoxScale, collidersInsideOverlapBox, spawnRotation, spawnedObjectLayer);

            Debug.Log("number of colliders found " + numberOfCollidersFound);

            if (numberOfCollidersFound == 0)
            {
                return (hit.point + new Vector3(0, offsetFromGround, 0), spawnRotation);
            }
            else
            {
                return (Vector3.negativeInfinity, Quaternion.identity);
            }
        }
        else
        {
            return (Vector3.negativeInfinity, Quaternion.identity);
        }
    }
    [ServerCallback]
    internal static void SpawnCoin(Vector3 position)
    {
        var (pos,rot) = PositionRaycast(position);
        NetworkServer.Spawn(
            Object.Instantiate(MirrorNetworkManager.singleton.coinPrefab, pos, rot));
    }
}
