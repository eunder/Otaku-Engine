using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueParticleCollisionEvent : MonoBehaviour
{
    void OnParticleCollision(GameObject other)
    {
        if(other.GetComponent<PosterMeshCreator>())
        {
         EventActionManager.Instance.TryPlayEvent(other, "OnGlued");
        }
        if(other.GetComponent<BlockFaceTextureUVProperties>())
        {
         EventActionManager.Instance.TryPlayEvent(other, "OnGlued"); 
        }
    }
}