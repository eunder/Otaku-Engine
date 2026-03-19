using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalVariableEditModeWindowChecker : MonoBehaviour
{


    //used to show the local variable editor window in edit mode only

    public GameObject localVariableEditorWindow;
    // Start is called before the first frame update
    void Start()
    {
       if(EditModeStaticParameter.isInEditMode)
       {
            localVariableEditorWindow.SetActive(true);
       } 
    }

}
