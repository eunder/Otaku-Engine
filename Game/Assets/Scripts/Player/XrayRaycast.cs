using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XrayRaycast : MonoBehaviour
{
    private static XrayRaycast _instance;
    public static XrayRaycast Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        this.enabled = false;
    }


    public float size;

    public LayerMask layerMask;

    // Update is called once per frame
    void Update()
    {
        
        RaycastHit objectHitInfo;
        Ray ray = transform.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

              if (Physics.Raycast(ray, out objectHitInfo, Mathf.Infinity, layerMask))
              {
                    Vector2 view = transform.GetComponent<Camera>().WorldToViewportPoint(objectHitInfo.point);
                    Shader.SetGlobalVector("_position", view);

                    DynamicallyResizeSize(objectHitInfo.point);
              }

    }


    public float minSize = 1f; // Minimum size of _size
    public float maxSize = 10f; // Maximum size of _size

    public float maxDistance = 5f;

    void DynamicallyResizeSize(Vector3 hitPos)
    {

        float distance = Vector3.Distance(transform.position, hitPos);
        size = Mathf.Lerp(maxSize, minSize, distance/maxDistance);

        Shader.SetGlobalFloat("_sizeGlobalControl", size);
    } 

    void OnDisable()
    {
                Shader.SetGlobalFloat("_sizeGlobalControl", 0);
    }

}
