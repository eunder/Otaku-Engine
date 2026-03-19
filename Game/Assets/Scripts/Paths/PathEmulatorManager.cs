using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;
using DG.Tweening;

public class PathEmulatorManager : MonoBehaviour
{
    private static PathEmulatorManager _instance;
    public static PathEmulatorManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }



    public List<GameObject> allEmulatorObjects = new List<GameObject>();

    public GameObject pathEmulatorPrefab;


    public void CreatePathEmulatorObject(GameObject referenceObject, GameObject path, bool additive, bool rotationOnly, bool reverse)
    {
        if(referenceObject == null)
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>Reference object not found");
        }
        if(path == null)
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>Path object not found");
        }




        //remove all emulators on object "PlayOnPath"(so the movements dont stack and so that it may be reset quickly and easily)
        if(additive == false)
        {
            if(referenceObject.GetComponent<PathEmulatorList>() != null)
            {
                foreach(GameObject obj in referenceObject.GetComponent<PathEmulatorList>().pathEmulators)
                {
                    Destroy(obj);
                }
            }
        }



        GameObject newEmulatorPathObj = Instantiate(pathEmulatorPrefab, referenceObject.transform.position, Quaternion.identity);
 

        newEmulatorPathObj.GetComponent<PathEmulator>().referenceObject = referenceObject;
        newEmulatorPathObj.GetComponent<PathEmulator>().additive = additive;
        newEmulatorPathObj.GetComponent<PathEmulator>().rotationOnly = rotationOnly;
        newEmulatorPathObj.GetComponent<PathEmulator>().moveToPath = path.GetComponent<PathNode>().moveToPath;
        newEmulatorPathObj.GetComponent<PathEmulator>().reverse = reverse;


        //Update path or else error
        path.GetComponent<PathNode>().CreatePath();
        //sets the current splineMove object's path as the path of the object found
        newEmulatorPathObj.GetComponent<splineMove>().pathContainer = path.GetComponent<PathManager>();

    
        GlobalUtilityFunctions.AssignSplineMovePropertiesFromPathParameters(newEmulatorPathObj.GetComponent<splineMove>(), path.GetComponent<PathNode>());


        //Play
        newEmulatorPathObj.GetComponent<splineMove>().Stop();
        newEmulatorPathObj.GetComponent<splineMove>().reverse = reverse;
        newEmulatorPathObj.GetComponent<splineMove>().StartMove();

    }
}
