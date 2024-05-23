using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CarFollowCamera : NetworkBehaviour
{
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject UiGameObject;
    public float moveSmoothness = 1f;
    public float rotSmoothness = 1f;

    public Vector3 moveOffset;
    public Vector3 rotOffset;

    public Transform carTarget;

    void FixedUpdate()
    {
        //check for local player
        if(!isLocalPlayer) return;  

        _camera.SetActive(true);
        UiGameObject.SetActive(true);
        FollowTarget();
    }

    void FollowTarget()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        Vector3 targetPos = new Vector3();
        targetPos = carTarget.TransformPoint(moveOffset);
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, targetPos, moveSmoothness * Time.deltaTime);
    }

    void HandleRotation()
    {
        var direction = carTarget.position - _camera.transform.position;
        var rotation = new Quaternion();

        rotation = Quaternion.LookRotation(direction + rotOffset, Vector3.up);

        _camera.transform.rotation = Quaternion.Lerp(_camera.transform.rotation, rotation, rotSmoothness * Time.deltaTime);
    }

}
