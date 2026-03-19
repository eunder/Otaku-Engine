using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighLightManager : MonoBehaviour
{
    private static HighLightManager _instance;
    public static HighLightManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }







        private Material currentHighlightMat;
        public Material highlightMat_Basic;
        public Material highlightMat_DepthLayer;

        public Material highlightMat_Parent;
        public Material highlightMat_Child;


    public void SetHighLightMaterial(Material highlightMat)
    {
        currentHighlightMat = highlightMat;
    }


    public List<GameObject> highlightedObjectsInMap = new List<GameObject>();


    public void HighLightObjects(List<GameObject> objects, Color color)
    {
        foreach(GameObject obj in objects)
        {

                    //activate the block (if hidden)
                    obj.gameObject.SetActive(true);

                    //store original gameobject info (so that it can be set back later)
                    highlightedObjectsInMap.Add(obj);


                        Material[] newMatList = new Material[obj.GetComponentInChildren<Renderer>().materials.Length];
                        for(int i = 0; i < newMatList.Length; i++)
                        {
                            newMatList[i] = currentHighlightMat;
                        }
                           obj.GetComponentInChildren<Renderer>().sharedMaterials = newMatList;
                        
                    

                        //set color
                        for(int i = 0; i < obj.GetComponentInChildren<Renderer>().materials.Length; i++)
                        {
                            obj.GetComponentInChildren<Renderer>().materials[i].SetColor("_Color2",color);
                        }


                        /*
                        //if there is a texture property... asign texture
                        if(obj.GetComponentInChildren<Renderer>().materials[0].HasProperty("_MainTex2"))
                        {
                            //set the poster texture as the xray texture. (dont use this line if youre going with the simpler poster xray material)
                            obj.GetComponentInChildren<Renderer>().material.SetTexture("_MainTex2", highlightedObject.baseMatList[0].GetTexture("_MainTex") );
                        }
                            */
        } 
    }

    public void HighLightObject(GameObject obj, Color color)
    {
                    //activate the block (if hidden)
                    obj.gameObject.SetActive(true);

                    //store original gameobject info (so that it can be set back later)
                    highlightedObjectsInMap.Add(obj);


                        //set highlight materials
                        Material[] newMatList = new Material[obj.GetComponentInChildren<Renderer>().materials.Length];
                        for(int i = 0; i < newMatList.Length; i++)
                        {
                            newMatList[i] = currentHighlightMat;
                        }                 
                           obj.GetComponentInChildren<Renderer>().sharedMaterials = newMatList;

                        //set color
                        for(int i = 0; i < obj.GetComponentInChildren<Renderer>().materials.Length; i++)
                        {
                            obj.GetComponentInChildren<Renderer>().materials[i].SetColor("_Color2",color);
                        }

                        //if there is a texture property... asign texture
                        if(obj.GetComponentInChildren<Renderer>().materials[0].HasProperty("_MainTex2"))
                        {
                            //set the poster texture as the xray texture. (dont use this line if youre going with the simpler poster xray material)
                          //  obj.GetComponentInChildren<Renderer>().material.SetTexture("_MainTex2", highlightedObject.baseMatList[0].GetTexture("_MainTex") );
                        }


    }



    //WARNING AT THE MOMENT... THE PARAMETERS FOR THE UNHIGHLIGHT FUNCTIONS DO NOT WORK AT ALL, THE CODE BELOW ISNT ACCURATE


    public void UnHighLightObjects(List<GameObject> objects)
    {
        foreach(GameObject hl in highlightedObjectsInMap)
        {
            if(hl != null)
            {
                if(hl.GetComponent<GeneralObjectInfo>())
                {
                    hl.GetComponent<GeneralObjectInfo>().ResetMaterials();
                }
                    else if(hl.GetComponent<Poster_DepthStencilFrame>())
                {
                    hl.GetComponent<Poster_DepthStencilFrame>().SetMaterialFromReferencedPosterID_Wrapper();
                }
            }
        }

        highlightedObjectsInMap.Clear(); // DOES THIS CLEAR ALL THE MEMORY?
    }

    public void UnHighLightObject(GameObject obj)
    {
        foreach(GameObject hl in highlightedObjectsInMap)
        {
            if(hl != null)
            {
                if(hl.GetComponent<GeneralObjectInfo>())
                {
                    hl.GetComponent<GeneralObjectInfo>().ResetMaterials();
                }
                else if(hl.GetComponent<Poster_DepthStencilFrame>())
                {
                    hl.GetComponent<Poster_DepthStencilFrame>().SetMaterialFromReferencedPosterID_Wrapper();
                }
            }
        }

        highlightedObjectsInMap.Clear(); // DOES THIS CLEAR ALL THE MEMORY?

    }

}
