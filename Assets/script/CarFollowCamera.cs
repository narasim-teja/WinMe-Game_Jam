using UnityEngine;

public class CarFollowCamera : MonoBehaviour
{
    public Transform target; 
    public Vector3 offset = new Vector3(0f, 2f, -5f); 

    public float smoothSpeed = 1f; 

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            transform.LookAt(target);
        }
    }
}
