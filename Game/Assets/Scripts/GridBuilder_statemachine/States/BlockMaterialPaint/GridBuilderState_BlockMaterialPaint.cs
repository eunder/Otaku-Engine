using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilderState_BlockMaterialPaint  : IGridBuilderState  //divides 1 by the appropriate parent dimensions, sets uv offsets to zero
{
    public IGridBuilderState DoState(GridBuilderStateMachine gridBuilder)
    {
        if(gridBuilder.newStateOnceEvent == false)
        {
            OnEnter(gridBuilder);
            gridBuilder.newStateOnceEvent = true;
        }


        OnStay(gridBuilder);

            if(Input.GetMouseButtonDown(1))
            {
                OnExit(gridBuilder);
                return gridBuilder.GridBuilderStateIdle;
            }
           
        return gridBuilder.GridBuilderStateBlockMaterialPaint;
    }


    
        void OnEnter(GridBuilderStateMachine gridBuilder)
        {
              gridBuilder.mouseLooker.enabled = true;
              gridBuilder.grid.gameObject.SetActive(false);
              gridBuilder.EditorCube.SetActive(false);
        }   


       RaycastHit hit;


        void OnStay(GridBuilderStateMachine gridBuilder)
        {
  
         if(Input.GetMouseButtonDown(0))
        {
                            Ray ray = gridBuilder.mainCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridBuilder.collidableLayers_layerMask))
        {
            hit.transform.gameObject.GetComponentInChildren<Renderer>().material = gridBuilder.materialToApply;
            hit.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().materialName_y = gridBuilder.materialNameToApply;
        }
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

        }

}
