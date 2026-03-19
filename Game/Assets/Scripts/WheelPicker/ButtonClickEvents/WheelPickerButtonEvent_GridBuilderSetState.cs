using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPickerButtonEvent_GridBuilderSetState : MonoBehaviour
{
    GridBuilderStateMachine gridBuilderStateMachine;
     public enum gridBuilderStateTypes{Selection ,CubeCreator,CubeDeleter, CubeResizer, CubeEdgeEditor, TextureFit, TextureScale, TextureOffset, TextureRotation, MaterialPaint};
     public gridBuilderStateTypes gridBuilderStateType;

    void Start()
    {
        gridBuilderStateMachine = GameObject.Find("GridBuilderStateMachine").GetComponent<GridBuilderStateMachine>();
    }

    public void SetGridBuilderState()
    {
        if(gridBuilderStateType == gridBuilderStateTypes.Selection)
        {
        gridBuilderStateMachine.currentState = gridBuilderStateMachine.GridBuilderStateSelection;
        }
        if(gridBuilderStateType == gridBuilderStateTypes.CubeCreator)
        {
        gridBuilderStateMachine.currentState = gridBuilderStateMachine.GridBuilderStateCubeCreator;
        }
        if(gridBuilderStateType == gridBuilderStateTypes.CubeDeleter)
        {
        gridBuilderStateMachine.currentState = gridBuilderStateMachine.GridBuilderStateCubeDeleter;
        }        
        if(gridBuilderStateType == gridBuilderStateTypes.CubeResizer)
        {
        gridBuilderStateMachine.currentState = gridBuilderStateMachine.GridBuilderStateCubeResizer;
        }
        if(gridBuilderStateType == gridBuilderStateTypes.CubeEdgeEditor)
        {
        gridBuilderStateMachine.currentState = gridBuilderStateMachine.GridBuilderStateCubeEdgeEditor;
        }
        if(gridBuilderStateType == gridBuilderStateTypes.TextureFit)
        {
        gridBuilderStateMachine.currentState = gridBuilderStateMachine.GridBuilderState_TextureFaceFit;
        }
        if(gridBuilderStateType == gridBuilderStateTypes.TextureScale)
        {
        gridBuilderStateMachine.currentState = gridBuilderStateMachine.GridBuilderState_TextureFaceScale;
        }
        if(gridBuilderStateType == gridBuilderStateTypes.TextureOffset)
        {
        gridBuilderStateMachine.currentState = gridBuilderStateMachine.GridBuilderState_TextureFaceOffset;
        }
        if(gridBuilderStateType == gridBuilderStateTypes.TextureRotation)
        {
        gridBuilderStateMachine.currentState = gridBuilderStateMachine.GridBuilderState_TextureFaceRotation;
        }
        if(gridBuilderStateType == gridBuilderStateTypes.MaterialPaint)
        {
        gridBuilderStateMachine.currentState = gridBuilderStateMachine.GridBuilderStateBlockMaterialPaint;
        }

        //close wheel
        GameObject wheelPicker = GameObject.Find("CANVAS_WHEELPICKER");
        wheelPicker.GetComponent<WheelPickerHandler>().CloseWheelPicker();

        //makes sure player does not interactive with objects in the world while using these tools
        PlayerObjectInteractionStateMachine.Instance.enabled = false;

    }

}
