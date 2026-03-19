using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poster_DepthStencilFrame : MonoBehaviour
{
    public GameObject posterToReference;
    public PosterMeshCreator posterMeshCreatorReference;
    public SkinnedMeshRenderer skinMeshRenderer;


    public string IdOfPosterToReference;
    public float depth;
    public float size = 1;
    public float shapeKey_CurveX;
    public float shapeKey_CurveY;

    Material baseMat;
    bool posterFound = false;

    void Start()
    {
        baseMat = skinMeshRenderer.sharedMaterial;
    }

    //when the layer is created... this needs to be called or else the basic stencil rendering properties will not work
    public void AssignStencilMatSettings()
    {
        skinMeshRenderer.sharedMaterial.SetInt("_StencilComp", 3);
        skinMeshRenderer.sharedMaterial.SetFloat("_StencilRef", 1);
        skinMeshRenderer.sharedMaterial.SetFloat("_ZTest", 8);
        skinMeshRenderer.sharedMaterial.renderQueue = 3001;

        skinMeshRenderer.sharedMaterial.SetInt("_Fog", 0);
    }


        public void SetMaterialFromReferencedPosterID_Wrapper()
        {
            //first check if object is found
            bool objFound = false; 
            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if(go.name == IdOfPosterToReference)
                {
                    objFound = true;
                    break;
                }
            }   

            //if object is not found... then assign the base material...
            if(objFound)
            {
            StartCoroutine(SetMaterialFromReferencedPosterID());
            }
            else
            {
                skinMeshRenderer.sharedMaterial = baseMat;
            }
        }


        public IEnumerator SetMaterialFromReferencedPosterID()
    {

        if(!string.IsNullOrWhiteSpace(IdOfPosterToReference))
        //Find object
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if(go.name == IdOfPosterToReference)
            {
                //unsubscribe from previous event
                if(posterMeshCreatorReference != null)
                {
                posterMeshCreatorReference.OnSuccesfulMediaChange -= AssignMaterialOnMediaChange;
                }



                posterToReference = go;
                posterMeshCreatorReference = posterToReference.GetComponent<PosterMeshCreator>();
                
                //subscribe to event so that media changes whenever material on the reference poster changes
                posterMeshCreatorReference.OnSuccesfulMediaChange += AssignMaterialOnMediaChange;


                //so that the media gets assigned on poster reference select
                AssignMaterialOnMediaChange();

                break;
            }
        }

            yield return new WaitForSeconds(0);

    } 



    void AssignMaterialOnMediaChange()
    {
        Debug.Log("MATERIAL EVENT TRIGGERED");
        skinMeshRenderer.sharedMaterial = posterMeshCreatorReference.meshRenderer.sharedMaterial;
        AssignStencilMatSettings(); //change new mat settings

        PosterDepthLayerStencilRefManager.Instance.AssignCorrectStencilRefsToAllPostersInScene(); //so that the poster is assigned to its correct stencil ref

        UpdateGeneralValues(); //call this so that the depth layer is correctly resized on media change
    }



    //When and how will the aspect ration be updated?

    public void UpdateAspectRatio()
    {
        //ASPECT RATIO CORRECTION


        //set unique values just for depth stencil layers
        box = new Vector2(size,size);

        if(posterToReference != null)
        image =  posterToReference.GetComponent<PosterMeshCreator>().image;


        float dbl = image.x / image.y;
        if((box.y * dbl) <= box.x )
        {
            resizedImage = new Vector2((box.y * dbl), box.y);
        }
        else
        {
            resizedImage = new Vector2(box.x, box.x / dbl);
        }

    }


    public void UpdatePosition()
    {
        transform.localPosition = new Vector3(0f,-depth,0f);
    }

    public void ResizeImage()
    {
        UpdateAspectRatio();
        transform.localScale = new Vector3(resizedImage.x, 1, resizedImage.y);
    }

    public void UpdateShapeKeys()
    {
        skinMeshRenderer.SetBlendShapeWeight(0, shapeKey_CurveX * 100);
        skinMeshRenderer.SetBlendShapeWeight(1, shapeKey_CurveY * 100);
    }

    //IMPORTANT TO UPDATE BOUNDS WHEN THE BLENDERSHAPE IS CHANGED
    void UpdateBounds()
    {  
        //a bit unoptimzed... (it does not convert the scale of the bounds... only the center) but it will do for now
        Bounds bounds = skinMeshRenderer.localBounds;

        //use the center of the poster parent as the center of this object's bounds  (world -> local space)
        bounds.center = transform.InverseTransformPoint(transform.parent.GetComponent<MeshRenderer>().bounds.center);
        skinMeshRenderer.localBounds = bounds;
     }



    public Vector2 box = new Vector2(1,1);

     public Vector2 image = new Vector2(512,512); //this is what sizes the image
     Vector2 resizedImage;


    public void UpdateGeneralValues()
    {
                UpdatePosition();
                ResizeImage();
                UpdateShapeKeys();
                UpdateBounds();
                UpdateRenderOrder();
    }

    //updates render order so that the list order matters (starts from 3500 and subtracts from there)
    public void UpdateRenderOrder()
    {
            for(int i = 0;  i < GetComponentInParent<PosterDepthLayerList>().posterDepthLayerList.Count; i++)
            {
                GetComponentInParent<PosterDepthLayerList>().posterDepthLayerList[i].GetComponent<Poster_DepthStencilFrame>().skinMeshRenderer.sharedMaterial.renderQueue = 3500 - i;
            }
    }

    //MOVING LAYER UP/DOWN IN A LIST MECHANIC
    
    public void MoveLayerUpInList()
    {
        int indexOfThisFrame = GetComponentInParent<PosterDepthLayerList>().posterDepthLayerList.IndexOf(gameObject);


        if(indexOfThisFrame - 1 >= 0)
        {
            GameObject temp = GetComponentInParent<PosterDepthLayerList>().posterDepthLayerList[indexOfThisFrame];
            GetComponentInParent<PosterDepthLayerList>().posterDepthLayerList[indexOfThisFrame] = GetComponentInParent<PosterDepthLayerList>().posterDepthLayerList[indexOfThisFrame - 1];
            GetComponentInParent<PosterDepthLayerList>().posterDepthLayerList[indexOfThisFrame - 1] = temp;

            //update the depth layer list
            ItemEditStateMachine.Instance.UpdateDepthLayerList();
        }
    }
    public void MoveLayerDownInList()
    {
        int indexOfThisFrame = GetComponentInParent<PosterDepthLayerList>().posterDepthLayerList.IndexOf(gameObject);

        //if there is a index spot right above... then proceed
        if(GetComponentInParent<PosterDepthLayerList>().posterDepthLayerList.Count - 1  >= indexOfThisFrame + 1)
        {
            GameObject temp = GetComponentInParent<PosterDepthLayerList>().posterDepthLayerList[indexOfThisFrame];
            GetComponentInParent<PosterDepthLayerList>().posterDepthLayerList[indexOfThisFrame] = GetComponentInParent<PosterDepthLayerList>().posterDepthLayerList[indexOfThisFrame + 1];
            GetComponentInParent<PosterDepthLayerList>().posterDepthLayerList[indexOfThisFrame + 1] = temp;

            //update the depth layer list
            ItemEditStateMachine.Instance.UpdateDepthLayerList();
        }
    }



    //DEBUGGING PURPOSES!!!
    void Update()
    {

        if(posterMeshCreatorReference)
        {

            //if it is a gif, then constantly update the texture
            if(posterMeshCreatorReference.urlFilePath.Contains(".gif") || posterMeshCreatorReference.urlFilePath_eventModified.Contains(".gif"))
            {                                                         //you have to use getcomponent renderer here because the "currentmaterial" does not account for media swap event
                skinMeshRenderer.sharedMaterial.SetTexture("_MainTex", posterMeshCreatorReference.GetComponent<Renderer>().sharedMaterial.mainTexture);
            }

        //for some reason... the video render texture wont make it in time... unfortanately, you have to do this
        if(posterMeshCreatorReference.isVideo && skinMeshRenderer.sharedMaterial.mainTexture == null)
        {
            skinMeshRenderer.sharedMaterial.SetTexture("_MainTex", posterMeshCreatorReference.meshRenderer.sharedMaterial.mainTexture);
        }

        //DEBUG
        UpdateAspectRatio();
        }
    }


    

    void OnDestroy()
    {
        //unsuscribe the poster from the "MediaChange" event or else there WILL be errors!!!
                        //unsubscribe from previous event
        if(posterMeshCreatorReference != null)
        {
        posterMeshCreatorReference.OnSuccesfulMediaChange -= AssignMaterialOnMediaChange;
        }
    }




}
