using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class KartController : MonoBehaviour
{
    private bool isCarGrounded;
    public LayerMask groundLayer;

    private float moveInput;
    private float turnInput;

    public float airDrag;
    public float groundDrag;


    public float fwdSpeed;
    public float revSpeed;
    public float turnSpeed;

    public Rigidbody sphereRb;

    private void Start()
    {
        sphereRb.transform.parent = null;
    }
    private void Update()
    {
        moveInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");
        moveInput *= moveInput >0 ? fwdSpeed : revSpeed;   


        transform.position = sphereRb.transform.position;

        float newRotation = turnInput * turnSpeed * Time.deltaTime * Input.GetAxisRaw("Vertical");
        transform.Rotate(0, newRotation, 0);

        RaycastHit hit;
        isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit,1f , groundLayer);
    
        transform.rotation = Quaternion.FromToRotation(transform.up , hit.normal) * transform.rotation; 
        
         sphereRb.drag = isCarGrounded ? groundDrag : airDrag;
    }

    private void FixedUpdate()
    {
        if (isCarGrounded)
        {
            sphereRb.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
        }
        else
        {
            sphereRb.AddForce(transform.up * -1000f);
        }
            

    }

}
