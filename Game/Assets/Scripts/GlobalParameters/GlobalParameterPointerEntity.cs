using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;


//used to point to en entity inside of the saved global entity list
public class GlobalParameterPointerEntity : MonoBehaviour
{  


    public string idOfGlobalEntityToPointTo;
    public GameObject entityPointingTo;

    public GameObject returnGameObjectEntity()
    {
        entityPointingTo = GameObject.Find(idOfGlobalEntityToPointTo);
        return entityPointingTo;
    }

    public void AssignImageOfEntityToPointTo()
    {
        if(returnGameObjectEntity())
        {
        GetComponent<SpriteRenderer>().sprite = returnGameObjectEntity().GetComponent<SpriteRenderer>().sprite;

        infoAboutWidget = returnGameObjectEntity().GetComponent<WidgetInfo>();
        }
    }


    public WidgetInfo infoAboutWidget;
    public TextMeshProUGUI widgetInfo;

    public void Update()
    {
        if(widgetInfo)
        {
        widgetInfo.text = infoAboutWidget.info;
        }
    }

}