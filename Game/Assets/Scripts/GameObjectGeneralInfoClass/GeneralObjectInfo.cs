using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GeneralObjectInfo : MonoBehaviour
{
    public Vector3 position_original;
    public Vector3 position;
    public Quaternion rotation_original;
    public Quaternion rotation;
    public Vector3 scale_original = new Vector3(1,1,1);
    public Vector3 scale = new Vector3(1,1,1);
    public string id;
    public string idOfParent;
    public GameObject parentObject;
    public int childIndex;
    public Material[] baseMatList;



    public List<string> children = new List<string>();

    public List<GameObject> childrenObjects = new List<GameObject>();

    public bool isActive_original = true;
    public bool isActive = true;

    public bool colliderActive_original = true;
    public bool colliderActive = true;


    public string entityName; //the name of the type of entity (audio source, block, etc)
    public string subName; // the player-given optional name

    public bool canBeDeactivatedByEngine = false; //used to tell the game which gameobjects can be deactivated or not.

    //this event calls the certain function for each entity type that resets its paramters back. (before it was modified by events)
    public UnityEvent resetCustomObjectParametersEvent;


    public bool wasLoadedAdditive = false;


    public bool ignorePlayerClick = false;
    public bool ignorePlayer = false;

    public bool isTrigger = false;

    void Start()
    {
        UpdateBaseMaterialList();


        if(SaveAndLoadLevel.Instance.allLoadedGameObjects.Contains(gameObject) == false)
        {
            SaveAndLoadLevel.Instance.allLoadedGameObjects.Add(gameObject);
        }
    }

    public void ResetAllObjectParameters()
    {
        resetCustomObjectParametersEvent.Invoke();
        GetComponent<EventHolderList>().ResetEventsThatHaveTriggered();
    }




    public void UpdateID()
    {
        id = transform.gameObject.name;

        isActive = gameObject.activeSelf;
    }

    public void UpdateParent()
    {
        if(transform.parent != null)
        {
                        System.Guid guid;
            //so that container and stuff dosnt get set as the parent
            if(System.Guid.TryParse(transform.parent.name, out guid))
             idOfParent = transform.parent.name;
        }


        UpdateParentObject();
    }

    public void UpdateParentObject()
    {
        if(!string.IsNullOrEmpty(idOfParent))
        {
            foreach(GeneralObjectInfo e in GlobalUtilityFunctions.GetAllGeneralObjectInfoClassesInMap())
            {
                if(e.id == idOfParent)
                {
                    parentObject = e.transform.gameObject;
                }
            }
        }

    }


    public void ClearParent()
    {
        idOfParent = null;
        parentObject = null;
    }


    public void UpdatePosition()
    {
        //do not edit the position if its playing on path
        if(CheckIfObjectIsBeingPlayedOnPath())
        return;

        position = transform.localPosition;
        rotation = transform.localRotation;
        scale = transform.localScale;

        position_original = position;
        rotation_original = rotation;
        scale_original = scale;


        UpdateChildIndex();
    }

    public void ResetPosition()
    {
        position = position_original;
        rotation = rotation_original;
        scale = scale_original;
    
        transform.localPosition = position_original;
        transform.localRotation = rotation_original;
        transform.localScale = scale_original;
    }

    public void ResetVisibility()
    {
        gameObject.SetActive(isActive_original);
    }


    public void SetParentAccordingToParentID()
    {
        if(string.IsNullOrEmpty(idOfParent))
        {
            transform.SetParent(null);
            SetChildIndex();
            return;
        }

        //the code that actually parents to the appropriate object
        foreach(GeneralObjectInfo e in GlobalUtilityFunctions.GetAllGeneralObjectInfoClassesInMap())
        {
            if(idOfParent == e.id)
            {
                transform.SetParent(e.transform);
                SetChildIndex();
                return;
            }
        }

        //this part is important, without it... if the object has no parent... it will stay parented to stuff like the selection tool
        transform.SetParent(null);
        SetChildIndex();
    }


    public void UpdateChildrenInParent()
    {
        if(transform.parent != null && transform.parent.GetComponent<GeneralObjectInfo>())
        {
            transform.parent.GetComponent<GeneralObjectInfo>().UpdateChildren();
        }

    }

    public void UpdateChildren()
    {
        UpdateChildrenInParent();

        children.Clear();
        children = new List<string>();
        foreach (Transform child in transform)
        {
            if(child.GetComponent<GeneralObjectInfo>())
            children.Add(child.name);
        }



        UpdateChildrenObjects();
    }

    public void UpdateChildrenObjects()
    {
        childrenObjects.Clear();
        childrenObjects = new List<GameObject>();

        foreach(string child in children)
        {
            foreach(GeneralObjectInfo e in GlobalUtilityFunctions.GetAllGeneralObjectInfoClassesInMap())
            {
                if(e.id == child)
                {
                    childrenObjects.Add(e.transform.gameObject);
                }
            }
        }
    }

/*
    public void UpdateChildrenObjectListOrder()
    {

        childrenObjects.Sort(SortByIndex);


        children.Clear();
        children = new List<string>();

        foreach(GameObject child in childrenObjects)
        {
            children.Add(child.name);
        }
    }


    static int SortByIndex(GameObject p1, GameObject p2)
    {
        return p1.GetComponent<GeneralObjectInfo>().childIndex.CompareTo(p2.GetComponent<GeneralObjectInfo>().childIndex);
    }
*/


    public void AddThisChildToParent()
    {
            if(!string.IsNullOrEmpty(idOfParent))
            {
                foreach(GeneralObjectInfo e in GlobalUtilityFunctions.GetAllGeneralObjectInfoClassesInMap())
                {
                    if(e.id == idOfParent)
                    {
                        e.transform.GetComponent<GeneralObjectInfo>().children.Add(id);
                    }
                }

            }

    }

    public void RemoveThisChildFromParent()
    {
            if(!string.IsNullOrEmpty(idOfParent))
            {
                foreach(GeneralObjectInfo e in GlobalUtilityFunctions.GetAllGeneralObjectInfoClassesInMap())
                {
                    if(e.id == idOfParent)
                    {
                        e.transform.GetComponent<GeneralObjectInfo>().children.Remove(id);
                    }
                }
            }
    }
    


    public void UpdateChildIndex()
    {
        childIndex = transform.GetSiblingIndex();
    }


    public void SetChildIndex()
    {
        transform.SetSiblingIndex(childIndex);
    }




    public void DisableCollider()
    {
        GetComponent<Collider>().enabled = false;
    }



    public void EnableCollider()
    {
        GetComponent<Collider>().enabled = true;
    }


    public void ResetCollider()
    {
        colliderActive = colliderActive_original;
        GetComponent<Collider>().enabled = colliderActive;
    }



    public void UpdateBaseMaterialList()
    {
        if(GetComponent<Renderer>())
        {
        Material[] newMaterials = new Material[GetComponent<Renderer>().sharedMaterials.Length];

        // Copy the shared materials into the new array
        GetComponent<Renderer>().sharedMaterials.CopyTo(newMaterials, 0);

        baseMatList = newMaterials;
        }

    }



    public void ResetMaterials()
    {
        if(GetComponent<Renderer>())
        {
            if(GetComponent<Renderer>().sharedMaterials != baseMatList)
            {
            GetComponent<Renderer>().sharedMaterials = baseMatList;
            }
        }
    }


    public bool CheckIfObjectIsBeingPlayedOnPath()
    {
        bool beingPlayedOnPath = false;

        if(GetComponent<PathEmulatorList>())
        return true;


        bool hasChildBeingPlayedOnPath = CheckIfChildrenAreBeingPlayedOnPath(gameObject);
        bool hasParentBeingPlayedOnPath = CheckIfParentIsBeingPlayedOnPath(gameObject);

        if(hasChildBeingPlayedOnPath == true || hasParentBeingPlayedOnPath == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckIfParentIsBeingPlayedOnPath(GameObject obj)
    {
        if(obj == null)
            return false;

        if(obj.GetComponent<GeneralObjectInfo>().parentObject != null)
        {
            if(obj.GetComponent<GeneralObjectInfo>().parentObject.GetComponent<PathEmulatorList>())
            {
                return true;
            }
            else
            {
                return CheckIfParentIsBeingPlayedOnPath(obj.GetComponent<GeneralObjectInfo>().parentObject);
            }
        }

        return false;
    }


    public bool CheckIfChildrenAreBeingPlayedOnPath(GameObject obj)
    {
        foreach(GameObject child in obj.GetComponent<GeneralObjectInfo>().childrenObjects)
        {
            if(child.GetComponent<PathEmulatorList>())
            {
                return true;
            }

            return CheckIfChildrenAreBeingPlayedOnPath(child);    
        }

        return false;
    }


    public void UpdateGeneralObjectLayerProperties()
    {
        if(ignorePlayerClick)
        {
            gameObject.layer = 23;
        }
        else if (ignorePlayer)
        {
            gameObject.layer = 22;
        }
        else if (isTrigger)
        {
            gameObject.layer = 17;
        }
        else
        {
            gameObject.layer = 14;
        }
    }







    void OnDestroy()
    {
        if(SaveAndLoadLevel.Instance.allLoadedGameObjects.Contains(gameObject))
        SaveAndLoadLevel.Instance.allLoadedGameObjects.Remove(gameObject);
    }



}
