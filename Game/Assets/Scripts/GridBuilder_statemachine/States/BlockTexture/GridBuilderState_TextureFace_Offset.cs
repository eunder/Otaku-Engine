using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilderState_TextureFace_Offset : IGridBuilderState 
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
           
        return gridBuilder.GridBuilderState_TextureFaceOffset;
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
            switch(stateIndex)
            {
                case 0:
                gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = new Vector2(gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].x + 0.01f, gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].y);
                break;

                case 1:
                gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = new Vector2(gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].x, gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].y + 0.01f);
                break;

            }

            gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UpdateBlockUV();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            switch(stateIndex)
            {
                case 0:
                gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = new Vector2(gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].x - 0.01f, gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].y);
                break;

                case 1:
                gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[gridBuilder.blockQuadHighLighter.currentFaceHighlighting] = new Vector2(gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].x, gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UVOffSet[gridBuilder.blockQuadHighLighter.currentFaceHighlighting].y - 0.01f);
                break;

            }

            gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().UpdateBlockUV();
        } 



        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(stateIndex == 0)
            {
                stateIndex = 1;
                GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("X axis offset", UINotificationHandler.NotificationStateType.ping);

            }
            else
            {
                stateIndex = 0;
                GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("Y axis offset", UINotificationHandler.NotificationStateType.ping);
            }

        }

        //reset
        if(Input.GetKeyDown(KeyCode.R))
        {
            gridBuilder.blockQuadHighLighter.currenGameObjectHitting.GetComponentInChildren<BlockFaceTextureUVProperties>().ResetFaceOffset(gridBuilder.blockQuadHighLighter.currentFaceHighlighting);
            GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("Reseted uv offset", UINotificationHandler.NotificationStateType.ping);
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
