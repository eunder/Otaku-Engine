using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class GlobalParameterUIHolder : MonoBehaviour
{  
    
    public string idOfGlobalEntityToPointTo;

    public GameObject gameObjectEntity;

    public TextMeshProUGUI entityComment;
    public TextMeshProUGUI entityValue;


    public void AssignTheCurrentParameterWorkingWith()
    {
        GlobalParamaterInspector.Instance.currentEntityWorkingWith = gameObjectEntity;
    }


    //for inspector (when selecting which entity to point to) (global entity pointer)
    public void OnClickedToEditEntity()
    {
        GlobalParameterManager.Instance.currentSelectedObjID = gameObjectEntity.name;
        GlobalParameterManager.Instance.RefreshLocalAndGlobalVariableList();

        ItemEditStateMachine.Instance.currentObjectEditing = null;
        ItemEditStateMachine.Instance.currentObjectEditing = gameObjectEntity;

    }


    //for pointer entity editing
       public void OnClickedToSelectEntity()
    {
        ItemEditStateMachine.Instance.AssignCurrentlySelectedGlobalEntity(transform.gameObject);
    }

    //update the comment, parameter to show, etc.
    public void UpdateUIInfo()
    {
        entityComment.text = gameObjectEntity.GetComponent<Note>().note;

        if(gameObjectEntity.GetComponent<Counter>())
        {
            if(gameObjectEntity.GetComponent<Counter>().currentCount == gameObjectEntity.GetComponent<Counter>().currentCount_default)
            {
            entityValue.text = gameObjectEntity.GetComponent<Counter>().currentCount.ToString();
            }
            else
            {
            entityValue.text = "<color=yellow>" + gameObjectEntity.GetComponent<Counter>().currentCount;
            }
        }
        if(gameObjectEntity.GetComponent<StringEntity>())
        {
            if(gameObjectEntity.GetComponent<StringEntity>().currentString == gameObjectEntity.GetComponent<StringEntity>().currentString_default)
            {
            entityValue.text = gameObjectEntity.GetComponent<StringEntity>().currentString;
            }
            else
            {
            entityValue.text = "<color=yellow>" + gameObjectEntity.GetComponent<StringEntity>().currentString;
            }
        }
        if(gameObjectEntity.GetComponent<Date>())
        {
            entityValue.text = gameObjectEntity.GetComponent<Date>().date.ToString(); 
        }
    }


}