using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class treasure_box_script : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public GameObject treasure_box;
    public GameObject treasure_box_top;
    public GameObject treasure_box_cam;
    public ParticleSystem confeti_particle_effect1;
    public ParticleSystem confeti_particle_effect2;
    public ParticleSystem aura_particle_effect;
    Rigidbody treasure_box_rb;
    Rigidbody treasure_box_top_rb;

    public GameObject temporaryObject;
    void Start()
    {
        treasure_box_top_rb = treasure_box_top.GetComponent<Rigidbody>();
        StartCoroutine(Remove_treasure_box_top());
    }   

    // Update is called once per frame
    void Update()
    {  
        treasure_box.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    IEnumerator Remove_treasure_box_top()
    {
        yield return new WaitForSeconds(2f);
        treasure_box_top_rb.AddForce(Vector3.up * 15f, ForceMode.Impulse);

        ParticleSystem instance = Instantiate(confeti_particle_effect1);
        instance.Play();
        Destroy(instance.gameObject, instance.main.duration);

        ParticleSystem instance1 = Instantiate(confeti_particle_effect1);
        instance1.Play();
        Destroy(instance1.gameObject, instance1.main.duration);

        GameObject temp = Instantiate(temporaryObject, treasure_box_top.transform.position, Quaternion.identity,this.transform);
        temp.AddComponent<RotateObject>().rotationSpeed = 50f;
        Instantiate(aura_particle_effect,treasure_box_top.transform.position,Quaternion.identity,temp.transform);

        StartCoroutine(SmoothMove(temp.transform, temp.transform.position, temp.transform.position + new Vector3(0, 2f, 0), 1f));
        StartCoroutine(SmoothMove(treasure_box_cam.transform, treasure_box_cam.transform.position, treasure_box_cam.transform.position + new Vector3(0, 3f, -5f), 1f));
    }

    IEnumerator SmoothMove(Transform target, Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            target.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime/4;
            yield return null; 
        }
        target.position = end; 
    }
}
