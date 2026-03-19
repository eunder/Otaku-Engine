using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnURLInputFieldFinishEditing : MonoBehaviour
{
    public ItemEditStateMachine itemEditorStateMachine;

    public void onURLInputFieldFinishEditing()
    {
        if(!itemEditorStateMachine.currentObjectEditing.GetComponent<PosterMeshCreator>().urlFilePath.Equals(GetComponent<TMP_InputField>().text))
        {
       StartCoroutine(itemEditorStateMachine.currentObjectEditing.GetComponent<PosterMeshCreator>().LoadImage(GetComponent<TMP_InputField>().text));
        }
    }

}
