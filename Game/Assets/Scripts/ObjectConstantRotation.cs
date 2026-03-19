using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectConstantRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Vector3 rotateAxis = new Vector3(0,0,0);
    public float speed = 1.0f;
    // Update is called once per frame
    void Update()
    {
            transform.Rotate (rotateAxis * Time.deltaTime * speed); //rotates 50 degrees per second around z axis
    }
}
