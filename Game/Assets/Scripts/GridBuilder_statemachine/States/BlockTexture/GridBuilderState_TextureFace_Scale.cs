using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilderState_TextureFace_Scale : IGridBuilderState 
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
           
        return gridBuilder.GridBuilderState_TextureFaceScale;
    }


    
        void OnEnter(GridBuilderStateMachine gridBuilder)
        {
                    gridBuilder.mouseLooker.enabled = true;
              gridBuilder.grid.gameObject.SetActive(false);
              gridBuilder.EditorCube.SetActive(false);

            gridBuilder.blockQuadHighLighter.gameObject.SetActive(true);
            gridBuilder.toolTipText.text = "Scroll Wheel: Change Value | Shift: Change Mode | C: unlock mouse | R: Reset | <color=red> Q: go back";
        }   




        void OnStay(GridBuilderStateMachine gridBuilder)
        {


        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            if(stateIndex == 0)
            {
                gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = new Vector2(gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].x + 0.01f, gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].y);
            }
            else if(stateIndex == 1)
            {
                gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = new Vector2(gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].x, gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].y + 0.01f);
            }
            else if(stateIndex == 2)
            {
                gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = new Vector2(gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].x + 0.01f, gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].y + 0.01f);
            }

            gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UpdateBlockUV();
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            if(stateIndex == 0)
            {
                gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = new Vector2(gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].x - 0.01f, gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].y);
            }
            else if(stateIndex == 1)
            {
                gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = new Vector2(gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].x, gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].y - 0.01f);
            }
            else if(stateIndex == 2)
            {
                gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = new Vector2(gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].x - 0.01f, gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVScale[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].y - 0.01f);
            }

            gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UpdateBlockUV();
        } 



        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            stateIndex++;

            if(stateIndex > 2)
            stateIndex = 0;

            if(stateIndex == 0)
            {
                GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("X axis", UINotificationHandler.NotificationStateType.ping);
            }
            else if(stateIndex == 1)
            {
                GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("Y axis", UINotificationHandler.NotificationStateType.ping);
            }
            else if(stateIndex == 2)
            {
                GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("X and Y axis", UINotificationHandler.NotificationStateType.ping);
            }

        }



        //reset
        if(Input.GetKeyDown(KeyCode.R))
        {
            gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().ResetFaceScale(gridBuilder.blockQuadHighLighter.currentFaceHighlighting);
            GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("Reseted uv scale", UINotificationHandler.NotificationStateType.ping);
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
