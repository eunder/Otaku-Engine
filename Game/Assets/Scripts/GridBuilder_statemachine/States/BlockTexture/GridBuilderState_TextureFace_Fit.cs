using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilderState_TextureFace_Fit  : IGridBuilderState  //divides 1 by the appropriate parent dimensions, sets uv offsets to zero
{
    public IGridBuilderState DoState(GridBuilderStateMachine gridBuilder)
    {
        if(gridBuilder.newStateOnceEvent == false)
        {
            OnEnter(gridBuilder);
            gridBuilder.newStateOnceEvent = true;
        }


        OnStay(gridBuilder);

            if(Input.GetKeyDown(KeyCode.Q))
            {
                OnExit(gridBuilder);
                return gridBuilder.GridBuilderStateIdle;
            }
           
        return gridBuilder.GridBuilderState_TextureFaceFit;
    }


    
        void OnEnter(GridBuilderStateMachine gridBuilder)
        {
                    gridBuilder.mouseLooker.enabled = true;
              gridBuilder.grid.gameObject.SetActive(false);
              gridBuilder.EditorCube.SetActive(false);

            gridBuilder.blockQuadHighLighter.gameObject.SetActive(true);
            gridBuilder.toolTipText.text = "Click: Fit | C: unlock mouse | <color=red> Q: go back";

        }   




        void OnStay(GridBuilderStateMachine gridBuilder)
        {
  
         if(Input.GetMouseButtonDown(0))
        {
            if(gridBuilder.blockQuadHighLighter.currentFaceHighlighting == 0 || gridBuilder.blockQuadHighLighter.currentFaceHighlighting == 2)
            {
            gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = new Vector2(1/gridBuilder.blockQuadHighLighter.currenGameObjectHitting.transform.localScale.x,1/gridBuilder.blockQuadHighLighter.currenGameObjectHitting.transform.localScale.z );
            gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = new Vector2(0.0f,0.0f);
            }
            if(gridBuilder.blockQuadHighLighter.currentFaceHighlighting == 1 || gridBuilder.blockQuadHighLighter.currentFaceHighlighting == 3)
            {
            gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = new Vector2(1/gridBuilder.blockQuadHighLighter.currenGameObjectHitting.transform.localScale.z,1/gridBuilder.blockQuadHighLighter.currenGameObjectHitting.transform.localScale.y );
            gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = new Vector2(0.0f,0.0f);
            }
            if(gridBuilder.blockQuadHighLighter.currentFaceHighlighting == 4 || gridBuilder.blockQuadHighLighter.currentFaceHighlighting == 5)
            {
            gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = new Vector2(1/gridBuilder.blockQuadHighLighter.currenGameObjectHitting.transform.localScale.x,1/gridBuilder.blockQuadHighLighter.currenGameObjectHitting.transform.localScale.y );
            gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = new Vector2(0.0f,0.0f);
            }

            gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UpdateBlockUV();
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
                    Cursor.lockState = CursorLockMode.None;
                    gridBuilder.mouseLooker.enabled = false;
        }
        if(Input.GetKeyUp(KeyCode.C))
        {
                    Cursor.lockState = CursorLockMode.Locked;
                    gridBuilder.mouseLooker.enabled = true;
        }

        }


        


        void OnExit(GridBuilderStateMachine gridBuilder)
        {
            gridBuilder.blockQuadHighLighter.gameObject.SetActive(false);
        }

}
