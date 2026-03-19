using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class GlobalParamaterInspector : MonoBehaviour
{  

    private static GlobalParamaterInspector _instance;
    public static GlobalParamaterInspector Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }



    public GameObject globalEntityUIHolderPrefab;

    public List<GameObject> UIListOfGlobalEntities = new List<GameObject>();

    public GameObject currentEntityWorkingWith;

    public void PopulateGlobalEntityList()
    {
        //clear list
        foreach(GameObject obj in UIListOfGlobalEntities)
        {
            Destroy(obj);
        }
        UIListOfGlobalEntities.Clear();



        foreach(GameObject obj in GlobalParameterManager.Instance.ReturnObjectListOfGlobalEntitites())
        {
          //  GameObject newUIObject = Instantiate(globalEntityUIHolder);
          //  newUIObject.GetComponent<GlobalParamaterInspector>().idOfGlobalEntityToPointTo = obj.GetComponent<GeneralObjectInfo>().id;
          //  UIListOfGlobalEntities.Add(newUIObject);
        }

    }


}