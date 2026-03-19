using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosterDepthLayerStencilRefManager : MonoBehaviour
{
    
    private static PosterDepthLayerStencilRefManager _instance;
    public static PosterDepthLayerStencilRefManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    public void AssignCorrectStencilRefsToAllPostersInScene()
    {
        int refInd = 4; // have to use 4 because the others are being used??

        PosterMeshCreator[] allCurrentPostersInScene = FindObjectsOfType(typeof(PosterMeshCreator), true) as PosterMeshCreator[];
        for(int i = 0; i < allCurrentPostersInScene.Length; i++)
        {
            if(allCurrentPostersInScene[i].GetComponent<PosterDepthLayerList>().posterDepthLayerList.Count >= 1)
            {
                //asign the stencil ref to the poster window "write" shader
                allCurrentPostersInScene[i].GetComponent<Renderer>().sharedMaterial.SetFloat("_StencilRef", refInd);

                //asign the stencil ref to all of the poster depth layers' materials 
                foreach(GameObject depthLayerObj in allCurrentPostersInScene[i].GetComponent<PosterDepthLayerList>().posterDepthLayerList)
                {
                    depthLayerObj.GetComponent<Renderer>().sharedMaterial.SetFloat("_StencilRef", refInd);
                    depthLayerObj.GetComponent<Poster_DepthStencilFrame>().UpdateGeneralValues();

                }

            refInd++;
            }
        }
    }

}
