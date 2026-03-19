using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testFitObjectToCam : MonoBehaviour
{
    public GameObject objectToLook;
    public Camera cameraToUse;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float cameraDistance = 2.0f; // Constant factor
        Vector3 objectSizes = objectToLook.GetComponent<Collider>().bounds.max - objectToLook.GetComponent<Collider>().bounds.min;
        float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
        float cameraView = 2.0f * Mathf.Tan(0.95f * Mathf.Deg2Rad * cameraToUse.fieldOfView); // Visible height 1 meter in front
        float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
        distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object
        cameraToUse.transform.position = Vector3.Lerp(cameraToUse.transform.position, objectToLook.GetComponent<Collider>().bounds.center - distance * cameraToUse.transform.forward, Time.deltaTime * 5.0f);


        //rotation camera match opposite of object
        var dir = (objectToLook.transform.localPosition + -objectToLook.transform.up) - objectToLook.transform.localPosition; // (local pos + obj up)  - local pos
        var rot = Quaternion.LookRotation(dir, Vector3.up); //calc a rotation that
        cameraToUse.transform.rotation = Quaternion.Slerp(cameraToUse.transform.rotation, rot, Time.deltaTime * 5.0f);
    }
}
