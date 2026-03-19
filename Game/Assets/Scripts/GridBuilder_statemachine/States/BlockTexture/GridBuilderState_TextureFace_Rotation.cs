using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilderState_TextureFace_Rotation : IGridBuilderState 
{
    int stateIndex = 0; //0 = x-axis, 1 = y-axis

    public IGridBuilderState DoState(GridBuilderStateMachine gridBuilder)
    {
        if(gridBuilder.newStateOnceEvent == false)
        {
            OnEnter(gridBuilder);
            gridBuilder.newStateOnceEvent = true;
        }


        OnStay(gridBuilder);

            if(Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1))
            {
                OnExit(gridBuilder);
                return gridBuilder.GridBuilderStateIdle;
            }
           
        return gridBuilder.GridBuilderState_TextureFaceRotation;
    }


    
        void OnEnter(GridBuilderStateMachine gridBuilder)
        {
                    gridBuilder.mouseLooker.enabled = true;
              gridBuilder.grid.gameObject.SetActive(false);
              gridBuilder.EditorCube.SetActive(false);

            gridBuilder.blockQuadHighLighter.gameObject.SetActive(true);
            gridBuilder.toolTipText.text = "Scroll Wheel: Change Value | C: UnlockMouse | Shift: Change Mode | R: Reset | <color=red> Q: go back";
        }   




        void OnStay(GridBuilderStateMachine gridBuilder)
        {


        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            if(stateIndex == 0)
            {
                gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponent<BlockFaceTextureUVProperties>().UVRotation[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVRotation[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] + 1f;
            }
            else if(stateIndex == 1)
            {
                gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponent<BlockFaceTextureUVProperties>().UVRotation[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVRotation[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] + 15f;
            }


            gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponent<BlockFaceTextureUVProperties>().UpdateBlockUV();
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            if(stateIndex == 0)
            {
                gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponent<BlockFaceTextureUVProperties>().UVRotation[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVRotation[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] - 1f;
            }
            else if(stateIndex == 1)
            {
                gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponent<BlockFaceTextureUVProperties>().UVRotation[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVRotation[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] - 15f;
            }


            gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponent<BlockFaceTextureUVProperties>().UpdateBlockUV();
        } 



        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            stateIndex++;

            if(stateIndex > 1)
            stateIndex = 0;

            if(stateIndex == 0)
            {
                GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("Increment 1", UINotificationHandler.NotificationStateType.ping);
            }
            else if(stateIndex == 1)
            {
                GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("Increment 15", UINotificationHandler.NotificationStateType.ping);
            }


        }


        //reset
        if(Input.GetKeyDown(KeyCode.R))
        {
            gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponent<BlockFaceTextureUVProperties>().ResetFaceRotation(gridBuilder.blockQuadHighLighter.currentFaceHighlighting);
            GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("Reseted uv rotation", UINotificationHandler.NotificationStateType.ping);
        }


            //freelook
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
            Cursor.lockState = CursorLockMode.Locked;
            gridBuilder.mouseLooker.enabled = true;
        }

}
