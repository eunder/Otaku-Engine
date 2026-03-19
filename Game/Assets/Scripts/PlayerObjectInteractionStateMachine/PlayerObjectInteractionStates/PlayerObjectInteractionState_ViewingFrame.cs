using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerObjectInteractionState_ViewingFrame : IPlayerObjectInteractionState 
{
    //HOW ALL OF THIS WORKS

    //On Enter: the state sets the camera in front of the poster, however, this calcualtion is not 100% Accurate.
    //On Stay: 
    //Auto 

        bool entered = false;
        bool pressedToExit = false;
        float cameraNormalizedDistance = 1;

        bool mouseMovement = false; //used to tell the game when to switch between mouse and input key movement


    public IPlayerObjectInteractionState DoState(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(entered == false)
        {
            OnEnter(playerObjectInteraction);
            entered = true;
        }


        OnStay(playerObjectInteraction);

            if(pressedToExit == true)
            {
                OnExit(playerObjectInteraction);
                return playerObjectInteraction.PlayerObjectInteractionStateIdle;
            }
        return playerObjectInteraction.PlayerObjectInteractionStateViewingFrame;
    }


    
        void OnEnter(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        lastFrameAspect = 0;    //so the positioning is reset
    }

        //Used to find the Base Fit/Fill screen height
        float maxYOffset;
        float minYOffset = 0;


        float correctionOffset = 0.1f; //the margin amount. How accurate should the positioning stage be before continuing to the next step?


        //STATES: 0 = fade to black event, 1 = making sure 0 does not get called again, 2 = the main viewing logic

        //0 calls "OnScreenEnterFadeToBlackComplete". This functions does the math to set the "base" viewing offset for the camera. (fit/fill).
        //When "OnScreenEnterFadeToBlackComplete" is done... it fades out from black



        PosterViewSettings currentPosterViewSettings; 


        void OnStay(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
        Debug.Log("state index: " + playerObjectInteraction.posterViewingStateIndex);

        //THIS NEEDS TO STAY. Becuse the current state machine state cannot be "entered" again...
        if(playerObjectInteraction.posterViewingStateIndex ==0)
        {
        playerObjectInteraction.posterViewingStateIndex = 1;
        //initiate fade to black
        playerObjectInteraction.fadeToBlack_Image.DORewind();
        playerObjectInteraction.fadeToBlack_Image.DOFade(1.0f, playerObjectInteraction.fadeToBlack_Duration).OnComplete(() =>OnScreenEnterFadeToBlackComplete(playerObjectInteraction)).SetEase(Ease.Linear);
        }








if(playerObjectInteraction.posterViewingStateIndex == 2 || playerObjectInteraction.posterViewer_continueViewing) // the second check is used just for making sure player can move while fading-out of a poster... when already viewing one 
{
        //ran every frame! a bit unoptimized but its a duct tape quick fix for the problem of the base viewing position logic running before the video even started
        CalculateBaseViewingPosition(playerObjectInteraction);

      
        //NOTE!!! the drawing order for a posters vertices is 0 (top right), 1 (bottom right), 2(bottom left), 3(top left)            VERY IMPORTANT TO GET RIGHT!!!
        Vector3 worldSpaceVertex_TopLeft = playerObjectInteraction.currentGameObjectLookingAt.transform.TransformPoint(playerObjectInteraction.currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[3]);
        Vector3 worldSpaceVertex_BottomLeft = playerObjectInteraction.currentGameObjectLookingAt.transform.TransformPoint(playerObjectInteraction.currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[2]);
        Vector3 worldSpaceVertex_TopRight = playerObjectInteraction.currentGameObjectLookingAt.transform.TransformPoint(playerObjectInteraction.currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[0]);
        Vector3 worldSpaceVertex_BottomRight = playerObjectInteraction.currentGameObjectLookingAt.transform.TransformPoint(playerObjectInteraction.currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[1]);
   


        //IMAGE PANNING WITHIN FRAME (figuring out the min/max ranges)
        //--------------------------------------------
        //NOTE. It may seem a bit confusing... Since the camera is parented to the poster when viewing... The z axis is the up/down. The y axis is the height



        //x pos. convert it from world point, to local point

        //give you the left edge local position of the poster
        float left_edge_pos = playerObjectInteraction.currentGameObjectLookingAt.transform.InverseTransformPoint(worldSpaceVertex_TopLeft).x; 

        //give you the bottom edge local position of the poster
        float bottom_edge_pos = playerObjectInteraction.currentGameObjectLookingAt.transform.InverseTransformPoint(worldSpaceVertex_BottomLeft).z;


        //important! use the local position of the camera MINUS the distanceFromWall of the poster mesh creator to find the correct zdepth
        float zDistanceToFaceOfPoster = playerObjectInteraction.playerCameraBase.transform.localPosition.y - playerObjectInteraction.currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().distanceFromWall;
      
        // get the distance between the left and right edges of the camera frustrum (making sure you use zDistanceToFaceOfPoster or else it will calculate the incorrect depth)
        float camWidth = Vector3.Distance(
        playerObjectInteraction.playerCamera.ViewportToWorldPoint(new Vector3(0,0, zDistanceToFaceOfPoster))  //bottom left edge pos
        ,
        playerObjectInteraction.playerCamera.ViewportToWorldPoint(new Vector3(1,0, zDistanceToFaceOfPoster))  //bottom right edge pos 
        );

        // ...same thing but with bottom and top edges to get height
      float camHeight = Vector3.Distance( 
        playerObjectInteraction.playerCamera.ViewportToWorldPoint(new Vector3(0,0, zDistanceToFaceOfPoster))    //bottom left edge pos
        , 
        playerObjectInteraction.playerCamera.ViewportToWorldPoint(new Vector3(0,1, zDistanceToFaceOfPoster))    //top left edge pos
        );

        //Calculate the min and max positions... This is done by getting the starting edge then adding half of the camera height.
        //the max can simply be found by using the positive opposite of the min. However. This all depends of the min being negative(zoomed into the poster)


        //POSTER EDGE CALCUATIONS
        float cameraWidthOffset = 0;
        float cameraHeightOffset = 0;
        //If the zoomedoffset is zoomed out... then allow the camera to move to corners
        //NOTE: leave out the (camWidth/2) if you want the camera to reach the poster edge (frame visible)
        if(currentPosterViewSettings.zoomForcedOffset <= 0)
        {
            cameraWidthOffset = camWidth/2;
            cameraHeightOffset = camHeight/2;
        }                                               
                                                   //add the extra border parameter: find the size difference between left and right edges... then multiply it by "extraBorder"
        float xmin = left_edge_pos + (camWidth/2) - (1 * currentPosterViewSettings.extraBorder);   //for extra border relative to frame aspect: (Mathf.Abs(left_edge_pos - Mathf.Abs(left_edge_pos))
        float xmax = Mathf.Abs(xmin) - 0.001f; //FOR SOME REASON... YOU HAVE TO ADD THE "- 0.001f OR ELSE WHEN REACHING THE EDGE MAX... IT WILL RESET TO THE CENTER. (DUCT TAPE FIXED!)

        float zmin = bottom_edge_pos + (camHeight/2)  - (1 * currentPosterViewSettings.extraBorder);  //for extra border relative to frame aspect: (Mathf.Abs(bottom_edge_pos - Mathf.Abs(bottom_edge_pos))
        float zmax = Mathf.Abs(zmin) - 0.001f; //FOR SOME REASON... YOU HAVE TO ADD THE "- 0.001f OR ELSE WHEN REACHING THE EDGE MAX... IT WILL RESET TO THE CENTER. (DUCT TAPE FIXED!)


        //Debug
     //   Debug.Log("cam width: " + camWidth);
     //   Debug.Log("cam height: " + camHeight);
     //   Debug.Log("xmin: " + xmin);
     //   Debug.Log("xmax: " + xmax);
     //   Debug.Log("zmin: " + zmin);
     //   Debug.Log("zmax: " + zmax);

        
        //POSITION THE CAMERA USING THE MIN AND MAX RANGES
        //-----------------------------------------------------


        //position the camera using the input normalized value
        if(currentPosterViewSettings.scrollingMode == "delta")
        {
                
            //MOUSE DELTA
            Vector3 mouseScreenPosition = Input.mousePosition;
            Vector3 mouseViewportPosition = playerObjectInteraction.playerCamera.ScreenToViewportPoint(mouseScreenPosition);
            //inverse look if applicable
            if(currentPosterViewSettings.inverseLook)
            { 
                mouseViewportPosition = new Vector3(1 - mouseViewportPosition.x, 1 - mouseViewportPosition.y, 1 - mouseViewportPosition.z);
            }
                
            
            //INPUT TYPE DETECTION. if the player moves the mouse past a certain threshold... then use the mouseMovement logic

            if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0.2f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.2f)
            {
                mouseMovement = true;
            }

                                                                                            //dont accept input axis input if the dialogue box is open
            if((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && DialogueChoiceManager.Instance.dialogueChoiceBoxIsActive == false)
            {
                mouseMovement = false;
            }

            if(mouseMovement)
            {
                //Lerp the mouse movement
                Vector3 posBasedOnMouse = new Vector3( Mathf.Lerp(xmin, xmax, mouseViewportPosition.x),playerObjectInteraction.playerCameraBase.transform.localPosition.y,  Mathf.Lerp(zmin, zmax, mouseViewportPosition.y));
                playerObjectInteraction.playerCameraBase.transform.localPosition = Vector3.Lerp(playerObjectInteraction.playerCameraBase.transform.localPosition, posBasedOnMouse, 2.0f * Time.deltaTime) ;
            }
            else
            {
                float horizontalInput = (Input.GetAxis("Horizontal") + 1f) / 2f;
                float verticalInput = (Input.GetAxis("Vertical") + 1f) / 2f;
                //inverse look if applicable
                if(currentPosterViewSettings.inverseLook)
                {    
                    horizontalInput = 1 - horizontalInput;
                    verticalInput = 1 - verticalInput;
                }
                Vector3 calcPos = new Vector3( Mathf.Lerp(xmin, xmax, horizontalInput),playerObjectInteraction.playerCameraBase.transform.localPosition.y, Mathf.Lerp(zmin, zmax, verticalInput));
                playerObjectInteraction.playerCameraBase.transform.localPosition = Vector3.Lerp(playerObjectInteraction.playerCameraBase.transform.localPosition, calcPos, 2.0f * Time.deltaTime);
            }
        }

        //position the camera via slow scrolling
        if(currentPosterViewSettings.scrollingMode == "pan")
        {
        //inverse look if applicable
        if(currentPosterViewSettings.inverseLook)
        {    
            playerObjectInteraction.playerCameraBase.transform.localPosition = new Vector3( Mathf.Lerp(xmin, xmax, 1 - playerObjectInteraction.panXPos),playerObjectInteraction.playerCameraBase.transform.localPosition.y, Mathf.Lerp(zmin, zmax, 1 - playerObjectInteraction.panYPos));
        }
        else
        {
            playerObjectInteraction.playerCameraBase.transform.localPosition = new Vector3( Mathf.Lerp(xmin, xmax, playerObjectInteraction.panXPos),playerObjectInteraction.playerCameraBase.transform.localPosition.y, Mathf.Lerp(zmin, zmax, playerObjectInteraction.panYPos));
        }
        }


        //CAMERA ROTATION EFFECT
        //---------------------------------

        //1. Get the current normal values based on the min/max and current value using "InverseLerp"
        //2. Then... figure out the rotation offset amount to apply using Lerp. Range (-1 to 1)

        float currentNormalizedXValue = Mathf.InverseLerp(xmin, xmax, playerObjectInteraction.playerCameraBase.transform.localPosition.x);
        float rotOffsetX = Mathf.Lerp(-1, 1, currentNormalizedXValue) * currentPosterViewSettings.rotationEffectAmount * 10 * -1;

        float currentNormalizedZValue = Mathf.InverseLerp(zmin, zmax, playerObjectInteraction.playerCameraBase.transform.localPosition.z);
        float rotOffsetZ = Mathf.Lerp(-1, 1, currentNormalizedZValue) * currentPosterViewSettings.rotationEffectAmount * 10;

        //Create a vector 3 with the offsets to assign
        playerObjectInteraction.imageViewing_rotationOffset = new Vector3(rotOffsetZ, rotOffsetX,0);

        //Set the rotation back or else the rotation will keep being added
        Quaternion desiredRotation = Quaternion.Euler(90f, 0f, 0f);
        playerObjectInteraction.playerCameraBase.transform.localRotation = desiredRotation;

        //Add the rotation offset
        playerObjectInteraction.playerCameraBase.transform.localRotation *= Quaternion.Euler(playerObjectInteraction.imageViewing_rotationOffset);



        //CENTERING OF CAMERA DUE TO CAMERA FRUSTRUM GOING PAST CERTAIN BOUNDARIES
        //-----------------------------------------------------------------------------

        //if the local frustrum right edge position goes past the bottom right vertex local position(most likely due to zooming). Then set the x position to 0
        float viewportLocal_BottomRightEdgePos = playerObjectInteraction.currentGameObjectLookingAt.transform.InverseTransformPoint(playerObjectInteraction.playerCamera.ViewportToWorldPoint(new Vector3(1,0, zDistanceToFaceOfPoster))).x;
        float localPos_OfBottomRightVertex = playerObjectInteraction.currentGameObjectLookingAt.transform.InverseTransformPoint(worldSpaceVertex_BottomRight).x;
                                                                                //plus... that extra border
        if(viewportLocal_BottomRightEdgePos > localPos_OfBottomRightVertex + (1 * currentPosterViewSettings.extraBorder))
        {
        playerObjectInteraction.playerCameraBase.transform.localPosition = new Vector3( 0f ,playerObjectInteraction.playerCameraBase.transform.localPosition.y, playerObjectInteraction.playerCameraBase.transform.localPosition.z);
        }

        //if the local frustrum right edge position goes past the bottom right vertex local position(most likely due to zooming). Then set the z position to 0
        float viewportLocal_TopLeftEdgePos = playerObjectInteraction.currentGameObjectLookingAt.transform.InverseTransformPoint(playerObjectInteraction.playerCamera.ViewportToWorldPoint(new Vector3(0,1, zDistanceToFaceOfPoster))).z;
        float localPos_OfTopLeftVertex = playerObjectInteraction.currentGameObjectLookingAt.transform.InverseTransformPoint(worldSpaceVertex_TopLeft).z;
        if(viewportLocal_TopLeftEdgePos > localPos_OfTopLeftVertex + (1 * currentPosterViewSettings.extraBorder))
        {
        playerObjectInteraction.playerCameraBase.transform.localPosition = new Vector3( playerObjectInteraction.playerCameraBase.transform.localPosition.x , playerObjectInteraction.playerCameraBase.transform.localPosition.y, 0);
        }
        



        //INPUT CONTROLLS
        //----------------------------------



        //move left and right
        if(Input.GetKey(KeyCode.A) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
        {
            playerObjectInteraction.panXPos -= Time.deltaTime * 0.5f;
        }
        if(Input.GetKey(KeyCode.D) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
        {
            playerObjectInteraction.panXPos += Time.deltaTime * 0.5f;
        }
        //move up and down
        if(Input.GetKey(KeyCode.W) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
        {
            playerObjectInteraction.panYPos += Time.deltaTime * 0.5f;
        }
        if(Input.GetKey(KeyCode.S) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
        {
            playerObjectInteraction.panYPos -= Time.deltaTime * 0.5f;
        }

        //Clamp the panning values
        playerObjectInteraction.panXPos = Mathf.Clamp(playerObjectInteraction.panXPos, 0, 1);
        playerObjectInteraction.panYPos = Mathf.Clamp(playerObjectInteraction.panYPos, 0, 1);






        //ZOOMING
        if(currentPosterViewSettings.canZoom)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false) // forward
            {
                cameraNormalizedDistance -= 0.1f;
                float yPos = Mathf.Lerp(minYOffset, maxYOffset, cameraNormalizedDistance);
                playerObjectInteraction.playerCameraBase.transform.localPosition = new Vector3(playerObjectInteraction.playerCameraBase.transform.localPosition.x, yPos, playerObjectInteraction.playerCameraBase.transform.localPosition.z);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false) // backwards
            {
                cameraNormalizedDistance += 0.1f;
                float yPos = Mathf.Lerp(minYOffset, maxYOffset, cameraNormalizedDistance);
                playerObjectInteraction.playerCameraBase.transform.localPosition = new Vector3(playerObjectInteraction.playerCameraBase.transform.localPosition.x, yPos, playerObjectInteraction.playerCameraBase.transform.localPosition.z);
            }
        }
        else
        {
            cameraNormalizedDistance = 1;
        }
        //clamp camera distance
        cameraNormalizedDistance = Mathf.Clamp(cameraNormalizedDistance, 0.75f, 1);


        //APPLY FORCED ZOOM RATIO
        float cameraNormalizedDistancePlusForcedOffset = cameraNormalizedDistance + currentPosterViewSettings.zoomForcedOffset;
        float yPosScaled = minYOffset + ((cameraNormalizedDistancePlusForcedOffset - 0.5f) * 2 * (maxYOffset - minYOffset));
        playerObjectInteraction.playerCameraBase.transform.localPosition = new Vector3(playerObjectInteraction.playerCameraBase.transform.localPosition.x, yPosScaled, playerObjectInteraction.playerCameraBase.transform.localPosition.z);







        //ALLOW THE PLAYER TO CLICK WITH CURSOR
            if(Input.GetMouseButtonDown(0) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false && playerObjectInteraction.clickThrough_enabled)
           {
                Ray ray = playerObjectInteraction.gameObject.transform.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out playerObjectInteraction.objectHitInfo, Mathf.Infinity, playerObjectInteraction.painter_collidableLayers_layerMask))
                {  
                    if(DialogueBoxStateMachine.Instance.dialogueObjectList.Count <= 0 && DialogueChoiceManager.Instance.dialogueChoiceBoxIsActive == false)
                    {
                        GlobalUtilityFunctions.EvaluateClickRange(playerObjectInteraction.objectHitInfo.distance, playerObjectInteraction.objectHitInfo.transform.gameObject);
                    }
                }
           }


            if(playerObjectInteraction.clickThrough_enabled)
            {
                Ray ray = playerObjectInteraction.gameObject.transform.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out playerObjectInteraction.objectHitInfo, Mathf.Infinity, playerObjectInteraction.painter_collidableLayers_layerMask))
                {  

                      if(GlobalUtilityFunctions.IsWithinClickRange(playerObjectInteraction.objectHitInfo.distance, playerObjectInteraction.objectHitInfo.transform.gameObject))
                      {
                          playerObjectInteraction.ClickerNotifier_gameObject.SetActive(true);
                          playerObjectInteraction.ClickerNotifier_gameObject.transform.position = playerObjectInteraction.objectHitInfo.point;
                      }
                      else
                      {
                            playerObjectInteraction.ClickerNotifier_gameObject.SetActive(false);
                      }

                }
                else
                {
                     playerObjectInteraction.ClickerNotifier_gameObject.SetActive(false);
                }
            }



}
        }

        void OnExit(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
                playerObjectInteraction.currentGameObjectLookingAt.GetComponent<GeneralObjectInfo>().ResetVisibility();


                pressedToExit = false;
                playerObjectInteraction.LeavePosterViewingMode();
        }




        void OnScreenEnterFadeToBlackComplete(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
            if(playerObjectInteraction.currentGameObjectLookingAt_newer)
            {
                playerObjectInteraction.currentGameObjectLookingAt = playerObjectInteraction.currentGameObjectLookingAt_newer;
            }


        if(playerObjectInteraction.currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().isVideo)
        {
            playerObjectInteraction.currentGameObjectLookingAt.GetComponent<VLC_MediaPlayer>().PlayVideo(playerObjectInteraction.currentGameObjectLookingAt);
        }



        currentPosterViewSettings = playerObjectInteraction.currentGameObjectLookingAt.GetComponent<PosterViewSettings>();

        
        //disable center camera dot
        playerObjectInteraction.centerDot_Canvas.SetActive(false);


        //event
        if(ItemEditStateMachine.Instance.currentState == ItemEditStateMachine.Instance.ItemEditStateMachineStateIdle)
        {
        EventActionManager.Instance.TryPlayEvent(playerObjectInteraction.currentGameObjectLookingAt, "OnViewPoster");
        }


        //activate poster if it is not active
        if(playerObjectInteraction.currentGameObjectLookingAt.activeSelf == false)
        {
        playerObjectInteraction.currentGameObjectLookingAt.SetActive(true);
        }

        playerObjectInteraction.mouseLooker.enabled = false;
        playerObjectInteraction.playerMover.enabled = false;


        //hide mouse but unlock it
        Cursor.lockState = CursorLockMode.Confined;

        if(ItemEditStateMachine.Instance.currentState == ItemEditStateMachine.Instance.ItemEditStateMachineStateIdle)
        {
                        //to prevent poster event from messing with player mouse while they are in the toolbar menu
            if(EscapeToggleToolBar.toolBarisOpened == false)
            {
                Cursor.visible = false;
            }
        }
        else
        {
        Cursor.visible = true;
        }



        //enable xray and allow the player to click through on "clickThrough_enabled"
        if(playerObjectInteraction.clickThrough_enabled)
        {
            XrayRaycast.Instance.enabled = true;
            playerObjectInteraction.currentGameObjectLookingAt.layer = 2;
            Cursor.visible = true;
        }
        else //else... set it so that there is not post processing
        {
          playerObjectInteraction.currentGameObjectLookingAt.layer = 18;

          foreach (Transform child in playerObjectInteraction.currentGameObjectLookingAt.transform)
         {
                 child.gameObject.layer = 18;
         }
        }






        //reset panning values
        playerObjectInteraction.panXPos = 0.5f;
        playerObjectInteraction.panYPos = 0.5f;


        playerObjectInteraction.toolTipText.text = "";


        //after these things are complete, proceed to calculationg the proper max height
        playerObjectInteraction.posterViewingStateIndex = 2;

        
        lastFrameAspect = 0; //so the positioning is reset



        //fade out from black
        playerObjectInteraction.fadeToBlack_Image.DOFade(0.0f, playerObjectInteraction.fadeToBlack_Duration).SetEase(Ease.InQuart);
        }



        //This is used to prevent the base rotation/position from being set every frame... it only gets set when the aspect ratio changes. (useful for videos that take a while to load)
        //Because if you update the base position/rotation every frame... the viewing mechanic will not work at all!
        private float lastFrameAspect;        

        void CalculateBaseViewingPosition(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
            //For calculating height/width of frame
            Vector3 worldSpaceVertex_TopLeft = playerObjectInteraction.currentGameObjectLookingAt.transform.TransformPoint(playerObjectInteraction.currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[3]);
            Vector3 worldSpaceVertex_BottomLeft = playerObjectInteraction.currentGameObjectLookingAt.transform.TransformPoint(playerObjectInteraction.currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[2]);
            Vector3 worldSpaceVertex_BottomRight = playerObjectInteraction.currentGameObjectLookingAt.transform.TransformPoint(playerObjectInteraction.currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[1]);

            float heightOfFrame = Vector3.Distance(worldSpaceVertex_BottomLeft, worldSpaceVertex_TopLeft);
            float widthOfFrame = Vector3.Distance(worldSpaceVertex_BottomLeft, worldSpaceVertex_BottomRight);



            float frustrumHeight = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * playerObjectInteraction.playerCamera.fieldOfView);
            float frustrumWide = frustrumHeight * playerObjectInteraction.playerCamera.aspect;
            
            float distance = 0;
            float frameAspect = widthOfFrame/heightOfFrame;
            frameAspect = Mathf.Round(frameAspect * 100f) / 100f; //round to prevent floating point problems
       
        if(currentPosterViewSettings.alignmentMode == "fit")
        {
            if(frameAspect >= playerObjectInteraction.playerCamera.aspect)
            {
            distance = widthOfFrame / frustrumWide;
            }
            else
            {
            distance = heightOfFrame / frustrumHeight;
            }
        }
        if(currentPosterViewSettings.alignmentMode == "fill")
        {
            if(frameAspect >= playerObjectInteraction.playerCamera.aspect)
            {
            distance = heightOfFrame / frustrumHeight;
            }
            else
            {
            distance = widthOfFrame / frustrumWide;
            }
        }


        distance += playerObjectInteraction.currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().distanceFromWall; // offset from object base

        playerObjectInteraction.playerCameraBase.transform.SetParent(playerObjectInteraction.currentGameObjectLookingAt.transform);


        if(frameAspect != lastFrameAspect)            
        {
            //NOTE!!! the main point that the camera base holder exists is so that the base holder can "shake" the camera without having to use code to add the positioning logic...
            playerObjectInteraction.playerCameraBase.transform.position = playerObjectInteraction.currentGameObjectLookingAt.transform.position - distance * -playerObjectInteraction.currentGameObjectLookingAt.transform.up;
            
            
            playerObjectInteraction.playerCamera.transform.localRotation = Quaternion.identity; //set the camera rotation to 0, so it alligns properly straight foreward from the base cam holder

            //setting these euler angles makes the camera parented to the poster, face at the poster
            Quaternion desiredRotation = Quaternion.Euler(90f, 0f, 0f);
            playerObjectInteraction.playerCameraBase.transform.localRotation = desiredRotation;


            maxYOffset = playerObjectInteraction.playerCameraBase.transform.localPosition.y;
        }
        lastFrameAspect = frameAspect;

        }
}
