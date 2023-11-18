using UnityEngine;

public class CarScaler : MonoBehaviour
{
    public float scaleSpeed = 0.1f;
    public float minYScale = 0.3f;
    public float maxYScale = 1f;

    void Update()
    {
        
        float scaleInput = Input.GetAxis("Mouse ScrollWheel");

        float scalex = transform.localScale.x + transform.localScale.x * scaleInput * scaleSpeed;
        float scaley = transform.localScale.y + transform.localScale.y * scaleInput * scaleSpeed;
        float scalez = transform.localScale.z + transform.localScale.z * scaleInput * scaleSpeed;


        
        if(scaley > Mathf.Ceil(minYScale*10f)/10f && scaley < Mathf.Floor(maxYScale*10f)/10f)
        {
            transform.localScale = new Vector3(scalex, scaley, scalez);
            this.GetComponent<carMovement2>().rayCastDistance += this.GetComponent<carMovement2>().rayCastDistance * scaleInput * scaleSpeed;
            //Debug.Log(this.GetComponent<carMovement2>().rayCastDistance);

        }
    }
}
