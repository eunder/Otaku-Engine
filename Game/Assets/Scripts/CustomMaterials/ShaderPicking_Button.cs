using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderPicking_Button : MonoBehaviour
{
    public string shaderName = "Shader Graphs/StandardTransparentDither";

    public void AssignShaderOnButtonPress()
    {
            if(ItemEditStateMachine.Instance.currentObjectEditing)
            {
                if(ItemEditStateMachine.Instance.currentObjectEditing.GetComponent<PosterMeshCreator>())
                {
                    ItemEditStateMachine.Instance.currentObjectEditing.GetComponent<PosterMeshCreator>().ChangeShaderOfPoster(shaderName);
                    ItemEditStateMachine.Instance.currentObjectEditing.GetComponent<PosterMeshCreator>().TriggerOnSuccesfulMediaChangeEvent();
                }
            }

            ShaderPickingManager.Instance.HighlightCurrentSelectedShader();
    }
}
