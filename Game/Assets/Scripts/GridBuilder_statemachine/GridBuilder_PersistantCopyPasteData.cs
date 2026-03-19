using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;


public class GridBuilder_PersistantCopyPasteData : MonoBehaviour
{
    private static GridBuilder_PersistantCopyPasteData _instance;
    public static GridBuilder_PersistantCopyPasteData Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }


    public SaveAndLoadLevel.LevelData copiedData;

    public int totalAmountOfCoroutinesBeingUsedForLoading = 0; //used to make sure player cant paste/duplicate until the previous process has finished!

    //public List<string> uniqueBlockAdressesBeingUsedBySelection = new List<string>();

    public void CopyListOfObjects(List<GameObject> objects)
    {
        /*
        //FOR ADDING THE CUSTOME MATERIALS BEING USED BY THE SELECTION

        //loop through each block to get ALL the adresses being used
        foreach(GameObject block in objects)
        {
            if(block.GetComponent<Block>())
            {
                uniqueBlockAdressesBeingUsedBySelection.Add(block.GetComponentInChildren<BlockFaceTextureUVProperties>().materialName_y);
                uniqueBlockAdressesBeingUsedBySelection.Add(block.GetComponentInChildren<BlockFaceTextureUVProperties>().materialName_yneg);
                uniqueBlockAdressesBeingUsedBySelection.Add(block.GetComponentInChildren<BlockFaceTextureUVProperties>().materialName_x);
                uniqueBlockAdressesBeingUsedBySelection.Add(block.GetComponentInChildren<BlockFaceTextureUVProperties>().materialName_xneg);
                uniqueBlockAdressesBeingUsedBySelection.Add(block.GetComponentInChildren<BlockFaceTextureUVProperties>().materialName_z);
                uniqueBlockAdressesBeingUsedBySelection.Add(block.GetComponentInChildren<BlockFaceTextureUVProperties>().materialName_zneg);
            }
        }

        //remove any duplicates in the list
        uniqueBlockAdressesBeingUsedBySelection = uniqueBlockAdressesBeingUsedBySelection.Distinct().ToList();

        //loop through each material, and add the ones that match the unique block being used. (add it to the list of passed "selected objects")
        foreach(MaterialMedia material in MaterialMedia_HolderSingleton.Instance.customMaterials)
        {
            if(uniqueBlockAdressesBeingUsedBySelection.Contains(material.materialName))
            {
                objects.Add(material.transform.gameObject);
            }
        }
        */




        //Finally, set(save) the objects to the persistent levelData class 
        copiedData = null;
        copiedData = new SaveAndLoadLevel.LevelData();




        //make sure to remove the parenting from the gymbal container and update the position... or else when duplicating, the positioning will be from the last selected object, rather than the current selected object (if chain duplicating)
        foreach(GameObject obj in objects)
        {
            if(obj)
            {
            if(obj.GetComponent<GeneralObjectInfo>())
            {
                obj.GetComponent<GeneralObjectInfo>().SetParentAccordingToParentID();
                obj.GetComponent<GeneralObjectInfo>().UpdatePosition();
            }


            //fixes bug with duplicating/pasting rotated blocks...
            //DONT have it in the save map function or else it will not work well when maps are saved while blocks are being played on a path...
            if(obj.GetComponent<BlockFaceTextureUVProperties>())
            {
                obj.GetComponent<BlockFaceTextureUVProperties>().SetPivotToBlockCenter();
                obj.transform.GetComponent<BlockFaceTextureUVProperties>().UpdateCornerPositions();
            }
            }
        }


        foreach(GameObject obj in objects)
        {
            if(obj)
            SaveMap.Instance.AddObjectToList(copiedData, obj);
        }
    }


    public void PasteListOfObjects()
    {
        StartCoroutine(LoadList());
    }


    public List<string> allObjectIDsInMapBeforeNewOnesAreCreated = new List<string>(); //used to check if an object in the map already has the same ID...

    IEnumerator LoadList()
    {
        totalAmountOfCoroutinesBeingUsedForLoading++;


        //remove all selected items

        HighLightManager.Instance.UnHighLightObjects(GridBuilderStateMachine.Instance.selectedObjects);

        foreach(GameObject selectedItem in GridBuilderStateMachine.Instance.selectedObjects)
        {
            if(selectedItem)
            {
            if(selectedItem.GetComponent<GeneralObjectInfo>())
            {
            selectedItem.GetComponent<GeneralObjectInfo>().SetParentAccordingToParentID();
            }
            }   
        }
        GridBuilderStateMachine.Instance.selectedObjects.Clear();

        //save all the current IDs so that later you may assign a new GUID if there are matching gameobject IDs in a map...
        allObjectIDsInMapBeforeNewOnesAreCreated.Clear();
        foreach(GameObject obj in SaveAndLoadLevel.Instance.allLoadedGameObjects)
        {
            if(obj)
            allObjectIDsInMapBeforeNewOnesAreCreated.Add(obj.GetComponent<GeneralObjectInfo>().id);
        }


        yield return SaveAndLoadLevel.Instance.LoadEntities(copiedData, 2);

        GiveNewlyCreatedObjectsNewHierchy();

        PostLoad();

        //NOTE! this stuff needs to be called here instead of in selection state... because the selection state does not wait for the entities to load!
        //if this stuff is not called here, then the objects will not be put inside the gymbal container...



        foreach(GameObject obj in SaveAndLoadLevel.Instance.recentlyLoadedGameObjectList)
        {
            if(obj != null)
            {
               GridBuilderStateMachine.Instance.selectedObjects.Add(obj);
            }
        }
        foreach(GameObject obj in GridBuilderStateMachine.Instance.selectedObjects)
        {
            if(obj)
            obj.transform.SetParent(GridBuilderStateMachine.Instance.gymbalArrowsContainer);
        }
        
    
        GridBuilderStateMachine.Instance.ResetGimbalOrientation();


    //UNCOMMENTED BECAUSE IT WAS CAUSING ERRORS WITH SHARED MATERIALS ON BLOCKS... still works without it anyway...

     //   HighLightManager.Instance.SetHighLightMaterial(HighLightManager.Instance.highlightMat_Basic);
     //   HighLightManager.Instance.HighLightObjects(GridBuilderStateMachine.Instance.selectedObjects, Color.green);



    totalAmountOfCoroutinesBeingUsedForLoading--;



    }


    // Custom struct to store old and new IDs
    [System.Serializable]

    public struct EntityIdPair
    {
        public string old_ID;
        public string new_ID;
    }

    public List<EntityIdPair> idMappings = new List<EntityIdPair>();


    void GiveNewlyCreatedObjectsNewHierchy()
    {
        idMappings = new List<EntityIdPair>();

        //this look complex but really its simple....

        //store the new/old id
        //give the recently loaded object a new id
        //loop through all the objects and check each one to see which ones had the old parent id (idOfParent)... if they do.. assign the new id

        foreach(GameObject obj in SaveAndLoadLevel.Instance.recentlyLoadedGameObjectList)
        {
            if(obj != null)
            {

                //only assign new GUID if there is an object already in the map that contains the same id...
                string newID;
                if(allObjectIDsInMapBeforeNewOnesAreCreated.Contains(obj.GetComponent<GeneralObjectInfo>().id))
                {
                     newID = System.Guid.NewGuid().ToString();
                }
                else
                {
                     newID = obj.GetComponent<GeneralObjectInfo>().id;
                }
                obj.name = newID;


                string oldID = obj.GetComponent<GeneralObjectInfo>().id;

                obj.GetComponent<GeneralObjectInfo>().UpdateID();


        
                foreach(GameObject obj2 in SaveAndLoadLevel.Instance.recentlyLoadedGameObjectList)
                {
                    if(obj2)
                    {
                        if(obj2.GetComponent<GeneralObjectInfo>().idOfParent == oldID)
                        {
                            obj2.GetComponent<GeneralObjectInfo>().idOfParent = newID;
                        }
                    }
                }

                //add to the dictionary to keep track of... so that events are re-wired in the newly spawned objects
                idMappings.Add(new EntityIdPair { old_ID = oldID, new_ID = newID });
            }
        }










        //update the material names of posters
        foreach(GameObject obj in SaveAndLoadLevel.Instance.recentlyLoadedGameObjectList)
        {
            if(obj != null)
            {
                //check if the object has an event list
                if(obj.GetComponent<PosterMeshCreator>())
                {
                    obj.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial.name = obj.name;
                }
            }
        }



        //update the id's of blocks and posters to point to the correct new source poster
        //The way this works... it checks the appropriate adresse(s) for each thing... "GetNewIdFromOldId" will return a new id if something was found...
        foreach(GameObject obj in SaveAndLoadLevel.Instance.recentlyLoadedGameObjectList)
        {
            if(obj != null)
            {
                if(obj.GetComponent<PosterMeshCreator>())
                {
                    obj.GetComponent<PosterMeshCreator>().urlFilePath = GetNewIdFromOldId(obj.GetComponent<PosterMeshCreator>().urlFilePath);
                }

                if(obj.GetComponent<BlockFaceTextureUVProperties>())
                {
                    obj.GetComponent<BlockFaceTextureUVProperties>().materialName_y = GetNewIdFromOldId(obj.GetComponent<BlockFaceTextureUVProperties>().materialName_y);
                    obj.GetComponent<BlockFaceTextureUVProperties>().materialName_yneg = GetNewIdFromOldId(obj.GetComponent<BlockFaceTextureUVProperties>().materialName_yneg);
                    obj.GetComponent<BlockFaceTextureUVProperties>().materialName_x = GetNewIdFromOldId(obj.GetComponent<BlockFaceTextureUVProperties>().materialName_x);
                    obj.GetComponent<BlockFaceTextureUVProperties>().materialName_xneg = GetNewIdFromOldId(obj.GetComponent<BlockFaceTextureUVProperties>().materialName_xneg);
                    obj.GetComponent<BlockFaceTextureUVProperties>().materialName_z = GetNewIdFromOldId(obj.GetComponent<BlockFaceTextureUVProperties>().materialName_z);
                    obj.GetComponent<BlockFaceTextureUVProperties>().materialName_zneg = GetNewIdFromOldId(obj.GetComponent<BlockFaceTextureUVProperties>().materialName_zneg);
                }
            }
        }




        //re-wire the newly pasted objects with the correct ids: events
        foreach(GameObject obj in SaveAndLoadLevel.Instance.recentlyLoadedGameObjectList)
        {
            if(obj != null)
            {
                if(obj.GetComponent<EventHolderList>())
                {
                    foreach(SaveAndLoadLevel.Event e in obj.GetComponent<EventHolderList>().events)
                    {
                            e.id = GetNewIdFromOldId(e.id);
                    }
                }
            }
        }

        //re-wire the newly pasted objects with the correct ids: depth layer references
        foreach(GameObject obj in SaveAndLoadLevel.Instance.recentlyLoadedGameObjectList)
        {
            if(obj != null)
            {
                if(obj.GetComponent<PosterMeshCreator>())
                {
                    foreach(GameObject depthFrame in obj.GetComponent<PosterDepthLayerList>().posterDepthLayerList)
                    {
                        if(depthFrame)
                        {
                            depthFrame.GetComponent<Poster_DepthStencilFrame>().IdOfPosterToReference = GetNewIdFromOldId(depthFrame.GetComponent<Poster_DepthStencilFrame>().IdOfPosterToReference);
                        }
                    }
                }
            }
        }
        // and refresh them..
        PosterDepthLayerStencilRefManager.Instance.AssignCorrectStencilRefsToAllPostersInScene();
        foreach(GameObject poster in SaveAndLoadLevel.Instance.allLoadedPosters)
        {
            if(poster != null)
            {
                foreach(GameObject depthLayer in poster.GetComponent<PosterDepthLayerList>().posterDepthLayerList)// a for loop is required due to complexity of algorithm
                {
                    if(depthLayer)
                    {
                        depthLayer.GetComponent<Poster_DepthStencilFrame>().SetMaterialFromReferencedPosterID_Wrapper();
                        depthLayer.GetComponent<Poster_DepthStencilFrame>().UpdateGeneralValues();
                    }
                }
            }
        }







        //re-apply the hierchy stuff
        foreach(GameObject obj in SaveAndLoadLevel.Instance.recentlyLoadedGameObjectList)
        {
            if(obj != null)
            {
                obj.GetComponent<GeneralObjectInfo>().SetParentAccordingToParentID();
                obj.GetComponent<GeneralObjectInfo>().UpdateParent();            
            }
        }


/*
        //add the newly spawned object to parent children list
        foreach(GameObject obj in SaveAndLoadLevel.Instance.recentlyLoadedGameObjectList)
        {
            if(obj != null)
            {
                if(obj.GetComponent<GeneralObjectInfo>().parentObject)
                {
                    obj.GetComponent<GeneralObjectInfo>().parentObject.GetComponent<GeneralObjectInfo>().children.Add(obj.name);
                    obj.GetComponent<GeneralObjectInfo>().parentObject.GetComponent<GeneralObjectInfo>().childrenObjects.Add(obj);
                }
            }
        }

*/

        //Update "original position"s 
        foreach(GameObject obj in SaveAndLoadLevel.Instance.recentlyLoadedGameObjectList)
        {
            if(obj != null)
            {
                if(obj.GetComponent<GeneralObjectInfo>())
                {
                    obj.GetComponent<GeneralObjectInfo>().ResetPosition();
                }
            }
        }


/*
                //Update "original position"s 
        foreach(GameObject obj in SaveAndLoadLevel.Instance.recentlyLoadedGameObjectList)
        {
            if(obj.GetComponent<GeneralObjectInfo>())
            {
                obj.GetComponent<GeneralObjectInfo>().UpdatePosition();
            }
        }
*/

        //set newly created objects as last in childIndex order
        foreach(GameObject obj in SaveAndLoadLevel.Instance.recentlyLoadedGameObjectList)
        {
            if(obj != null)
            {
                int largestIndex = 0;
                if(obj.GetComponent<GeneralObjectInfo>().parentObject)
                foreach(GameObject child in obj.GetComponent<GeneralObjectInfo>().parentObject.GetComponent<GeneralObjectInfo>().childrenObjects)
                {
                    if(child)
                    {
                        if(child.GetComponent<GeneralObjectInfo>().childIndex > largestIndex)
                        {
                            largestIndex = child.GetComponent<GeneralObjectInfo>().childIndex;
                        }
                    }
                }
                obj.GetComponent<GeneralObjectInfo>().childIndex = largestIndex + 1;
            }
        }

/*
        foreach(GameObject obj in SaveAndLoadLevel.Instance.allLoadedGameObjects)
        {
            obj.GetComponent<GeneralObjectInfo>().UpdateChildrenObjectListOrder();
        }
*/


        foreach(GeneralObjectInfo obj in GlobalUtilityFunctions.GetAllGeneralObjectInfoClassesInMap())
        {
            if(obj != null)
            {
                obj.SetParentAccordingToParentID();
                obj.UpdateChildren();            
            }
        }
        



    //update waypoints of all paths (due to upated hierchy)
    PathNode[] allCurrentPathsInScene = FindObjectsOfType(typeof(PathNode), true) as PathNode[];
    for(int i = 0; i < allCurrentPathsInScene.Length; i++)
    {
        allCurrentPathsInScene[i].CreatePath();
    }




    }


    // Method to get the new ID given an old ID
    //if no new id was found... then just return the old id
    public string GetNewIdFromOldId(string oldId)
    {
        foreach (var mapping in idMappings)
        {
            if (mapping.old_ID == oldId)
            {
                return mapping.new_ID;
            }
        }
        return oldId; 
    }


    public void PostLoad()
    {

        //VERY IMPORTANT! (fixed on 6/26/2024). Make sure these block functions get called AFTER the hierchies and resting positions are set... or else there will be vertex problems!
        foreach(GameObject obj in SaveAndLoadLevel.Instance.recentlyLoadedGameObjectList)
        {
            if(obj != null)
            {
                if(obj.GetComponent<BlockFaceTextureUVProperties>())
                {
                    obj.transform.GetComponent<BlockFaceTextureUVProperties>().UpdateCorners();
                    obj.GetComponent<BlockFaceTextureUVProperties>().SetPivotToBlockCenter();
                    obj.transform.GetComponent<BlockFaceTextureUVProperties>().UpdateBlockUV();

                }
            }

        }



        //loop through all posters and set the correct shared material based on id
        foreach(GameObject poster in SaveAndLoadLevel.Instance.allLoadedPosters)
        {
            if(poster)
            poster.GetComponent<PosterMeshCreator>().AssignSharedMaterialBasedOnGUID();
        }

        //loop through all posters and set the correct shared material based on id
        foreach(GameObject block in SaveAndLoadLevel.Instance.allLoadedBlocks)
        {
            if(block)
            block.GetComponent<BlockFaceTextureUVProperties>().AssignSharedMaterialBasedOnGUID();
        }



    }




}