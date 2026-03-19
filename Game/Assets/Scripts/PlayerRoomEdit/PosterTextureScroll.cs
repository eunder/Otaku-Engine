using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosterTextureScroll : MonoBehaviour
{
    public Vector2 scrollSpeed;
    public float offsetX;
    public float offsetY;

    public PosterMeshCreator poster;

    public MeshRenderer meshRenderer;

    // Update is called once per frame
     void Update()
     {
        //make sure to only run this if the poster is a source material
        if(poster.isGUIDreference == false)
        {
            if(meshRenderer)
            {
                offsetX = Time.time * scrollSpeed.x;
                offsetY = Time.time * scrollSpeed.y;

                meshRenderer.sharedMaterial.mainTextureOffset = new Vector2(offsetX, offsetY);
            }
            else
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }
        }
     }
}
