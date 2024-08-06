using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CarCameraManager : NetworkBehaviour
{
    [SerializeField] private GameObject camPrefab;
    [SerializeField] private GameObject playerNameText;
    private GameObject _camera;
    public float moveSmoothness = 2.5f;
    public float rotSmoothness = 4f;

    public Vector3 moveOffset;
    public Vector3 rotOffset;

    public Transform carTarget;

    private void Start()
    {
        if (isLocalPlayer)
        {
            _camera = Instantiate(camPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            DontDestroyOnLoad(_camera);
        }
    }


    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            FollowTarget();
            // rotateText();
        }
    }

    void FollowTarget()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        Vector3 targetPos = carTarget.TransformPoint(moveOffset);
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, targetPos, moveSmoothness * Time.deltaTime);
    }

    void HandleRotation()
    {
        var direction = carTarget.position - _camera.transform.position;
        var rotation = Quaternion.LookRotation(direction + rotOffset, Vector3.up);

        _camera.transform.rotation = Quaternion.Lerp(_camera.transform.rotation, rotation, rotSmoothness * Time.deltaTime);
    }

    void rotateText(){
        playerNameText.transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward, _camera.transform.rotation * Vector3.up);
    }

    
}
