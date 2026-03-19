using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewingFrameEmulator : MonoBehaviour
{
    //THIS IS USED TO "EMULATE" THE PLAYER VIEWING A FRAME...
    //IT USES A SEPERATE CAMERA AND POSTER TO ALLOW THE HUD STENCIL POSTER TO MOVE DYNAMICALLY


    //stuff thats in the state machine general handler
    public Camera imitationCamera;
    public GameObject currentGameObjectLookingAt;
    public PosterViewSettings currentPosterViewSettings;




    public Vector3 imageViewing_rotationOffset;


    //stuff thats in the "viewing frame" state
    float maxYOffset;
    float minYOffset = 0;

    float cameraNormalizedDistance = 1;


    public Vector3 initialCameraPos;
    public Quaternion initialCameraRot;

    public Vector3 initialPosterPos;
    public Quaternion initalPosterRot;

    public Vector3 cameraPosOffset;
    public Quaternion cameraRotOffset;

    int storedLayer; //store the layer to set it back later

    public void InitiateCamera(GameObject poster, int renderQue)
    {



        currentGameObjectLookingAt = poster;
        currentPosterViewSettings = poster.GetComponent<PosterViewSettings>();

        //activate poster if it is not active
        if(currentGameObjectLookingAt.activeSelf == false)
        currentGameObjectLookingAt.SetActive(true);



        //set the poster layer to "overlay"
        storedLayer = currentGameObjectLookingAt.layer;
        currentGameObjectLookingAt.layer = 24;

        //if it is a stencil(has children)... set the layers on the children too
         foreach (Transform child in currentGameObjectLookingAt.transform)
         {
            //but make sure it dosnt set gizmos to the layer!
            if(child.gameObject.layer == 8)
            {
                 child.gameObject.layer = 24;
            }
         }


        imitationCamera.transform.SetParent(currentGameObjectLookingAt.transform);
        imitationCamera.transform.localPosition = new Vector3(0,0,0);


        initialCameraPos = imitationCamera.transform.localPosition;
        initialCameraRot = imitationCamera.transform.localRotation;
        initialPosterPos = currentGameObjectLookingAt.transform.localPosition;
        initalPosterRot = currentGameObjectLookingAt.transform.localRotation;

        SetMaterialShaderProperties(renderQue);


        if(currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().isVideo)
        {
            currentGameObjectLookingAt.GetComponent<VLC_MediaPlayer>().PlayVideo(currentGameObjectLookingAt);
        }
        
        lastFrameAspect = 0;

    }

    public void SetMaterialShaderProperties(int renderQue)
    {
        //disable fog... and make the poster(s) render behind stuff

        //make sure it always renders the pixels behind other objects(Always) so that it can get sorted based on render que order 
        currentGameObjectLookingAt.GetComponent<Renderer>().sharedMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
        currentGameObjectLookingAt.GetComponent<Renderer>().sharedMaterial.SetInt("_Fog", 0);

        //set render que to this or else the fog will get messed up
        currentGameObjectLookingAt.GetComponent<Renderer>().sharedMaterial.renderQueue = renderQue;

    }


    public void RemoveCameraEmulator()
    {
        //set the layer of the poster back
        currentGameObjectLookingAt.layer = storedLayer;

        //if it is a stencil(has children)... set the layers on the children too
         foreach (Transform child in currentGameObjectLookingAt.transform)
         {
            if(child.gameObject.layer == 8)
            {
                child.gameObject.layer = storedLayer;
            }
         }


        //set render que of the poster back to 3000 (the standard for the game)
        currentGameObjectLookingAt.GetComponent<Renderer>().sharedMaterial.renderQueue = 3000;


        currentGameObjectLookingAt.GetComponent<GeneralObjectInfo>().ResetVisibility();


        Destroy(imitationCamera.gameObject);
        Destroy(gameObject);
    }


        float currentXVal = 0.5f ; //centered
        float currentYVal = 0.5f ;  //centered

    // Update is called once per frame
    void Update()
    {
        //ran every frame! a bit unoptimized but its a duct tape quick fix for the problem of the base viewing position logic running before the video even started
        CalculateBaseViewingPosition();

        //NOTE!!! the drawing order for a posters vertices is 0 (top right), 1 (bottom right), 2(bottom left), 3(top left)            VERY IMPORTANT TO GET RIGHT!!!
        Vector3 worldSpaceVertex_TopLeft = currentGameObjectLookingAt.transform.TransformPoint(currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[3]);
        Vector3 worldSpaceVertex_BottomLeft = currentGameObjectLookingAt.transform.TransformPoint(currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[2]);
        Vector3 worldSpaceVertex_TopRight = currentGameObjectLookingAt.transform.TransformPoint(currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[0]);
        Vector3 worldSpaceVertex_BottomRight = currentGameObjectLookingAt.transform.TransformPoint(currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[1]);





                //IMAGE PANNING WITHIN FRAME (figuring out the min/max ranges)
        //--------------------------------------------
        //NOTE. It may seem a bit confusing... Since the camera is parented to the poster when viewing... The z axis is the up/down. The y axis is the height



        //x pos. convert it from world point, to local point

        //give you the left edge local position of the poster
        float left_edge_pos = currentGameObjectLookingAt.transform.InverseTransformPoint(worldSpaceVertex_TopLeft).x; 

        //give you the bottom edge local position of the poster
        float bottom_edge_pos = currentGameObjectLookingAt.transform.InverseTransformPoint(worldSpaceVertex_BottomLeft).z;


        //important! use the local position of the camera MINUS the distanceFromWall of the poster mesh creator to find the correct zdepth
        float zDistanceToFaceOfPoster = imitationCamera.transform.localPosition.y - currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().distanceFromWall;
      
        // get the distance between the left and right edges of the camera frustrum (making sure you use zDistanceToFaceOfPoster or else it will calculate the incorrect depth)
        float camWidth = Vector3.Distance(
        imitationCamera.ViewportToWorldPoint(new Vector3(0,0, zDistanceToFaceOfPoster))  //bottom left edge pos
        ,
        imitationCamera.ViewportToWorldPoint(new Vector3(1,0, zDistanceToFaceOfPoster))  //bottom right edge pos 
        );

        // ...same thing but with bottom and top edges to get height
      float camHeight = Vector3.Distance( 
        imitationCamera.ViewportToWorldPoint(new Vector3(0,0, zDistanceToFaceOfPoster))    //bottom left edge pos
        , 
        imitationCamera.ViewportToWorldPoint(new Vector3(0,1, zDistanceToFaceOfPoster))    //top left edge pos
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
        float xmax = Mathf.Abs(xmin);


        float zmin = bottom_edge_pos + (camHeight/2)  - (1 * currentPosterViewSettings.extraBorder);  //for extra border relative to frame aspect: (Mathf.Abs(bottom_edge_pos - Mathf.Abs(bottom_edge_pos))
        float zmax = Mathf.Abs(zmin);


        //Debug
     //   Debug.Log("cam width: " + camWidth);
     //   Debug.Log("cam height: " + camHeight);
     //   Debug.Log("xmin: " + xmin);
     //   Debug.Log("xmax: " + xmax);
     //   Debug.Log("zmin: " + zmin);
     //   Debug.Log("zmax: " + zmax);

        
        //POSITION THE CAMERA USING THE MIN AND MAX RANGES
        //-----------------------------------------------------


// OVERLAY DELTA MOVEMENTTHIS WAS SCRAPPED BECAUSE IT WAS WAY TOO BUGGY AND HARD TO WORK WITH 
/* //SCRAPPED CAM MOVEMENT
        //position the camera using the input normalized value
        
        //MOUSE DELTA
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseViewportPosition = imitationCamera.ScreenToViewportPoint(mouseScreenPosition);



        float moveX = -Input.GetAxis("Mouse X") * 1;
        float moveY = -Input.GetAxis("Mouse Y") * 1;

        float addedXVal = 0.5f + moveX;
        float addedYVal = 0.5f + moveY;

        if(currentPosterViewSettings.inverseLook)
        {
         addedXVal = 0.5f - moveX;
         addedYVal = 0.5f - moveY;
        }
        else
        {
         addedXVal = 0.5f + moveX;
         addedYVal = 0.5f + moveY;
        }

        Debug.Log("currentXVal: " + currentXVal);
        Debug.Log("currentYVal: " + currentYVal);


        currentXVal = Mathf.Lerp(currentXVal, addedXVal, Time.deltaTime * 1) ;
        currentYVal = Mathf.Lerp(currentYVal, addedYVal, Time.deltaTime * 1) ;


        currentXVal = Mathf.Clamp(currentXVal, 0, 1);
        currentYVal = Mathf.Clamp(currentYVal, 0, 1);


        //Lerp the movement
       //SCRAPPED CAM MOVEMENT Vector3 posBasedOnMouse = new Vector3( Mathf.Lerp(xmin, xmax, currentXVal),imitationCamera.transform.localPosition.y,  Mathf.Lerp(zmin, zmax, currentYVal));
       //SCRAPPED CAM MOVEMENT  imitationCamera.transform.localPosition = Vector3.Lerp(imitationCamera.transform.localPosition, posBasedOnMouse, 2.0f * Time.deltaTime) ;
                
        //FOR DEBUGGING PURPOSES... USES THE EXACT MOUSE POSITION ON SCREEN INSTEAD      
        //imitationCamera.transform.localPosition = new Vector3( Mathf.Lerp(xmin, xmax, mouseViewportPosition.x),imitationCamera.transform.localPosition.y,  Mathf.Lerp(zmin, zmax, mouseViewportPosition.y));


        //CAMERA ROTATION EFFECT
        //---------------------------------

        //1. Get the current normal values based on the min/max and current value using "InverseLerp"
        //2. Then... figure out the rotation offset amount to apply using Lerp. Range (-1 to 1)

        float currentNormalizedXValue = Mathf.InverseLerp(xmin, xmax, imitationCamera.transform.localPosition.x);
        float rotOffsetX = Mathf.Lerp(-1, 1, currentNormalizedXValue) * currentPosterViewSettings.rotationEffectAmount * 10 * -1;

        float currentNormalizedZValue = Mathf.InverseLerp(zmin, zmax, imitationCamera.transform.localPosition.z);
        float rotOffsetZ = Mathf.Lerp(-1, 1, currentNormalizedZValue) * currentPosterViewSettings.rotationEffectAmount * 10;

        //Create a vector 3 with the offsets to assign
        imageViewing_rotationOffset = new Vector3(rotOffsetZ, rotOffsetX,0);

        //Set the rotation back or else the rotation will keep being added
        Quaternion desiredRotation = Quaternion.Euler(90f, 0f, 0f);
        //SCRAPPED CAM MOVEMENT imitationCamera.transform.localRotation = desiredRotation;

        //Add the rotation offset
        //SCRAPPED CAM MOVEMENT imitationCamera.transform.localRotation *= Quaternion.Euler(imageViewing_rotationOffset);



        //CENTERING OF CAMERA DUE TO CAMERA FRUSTRUM GOING PAST CERTAIN BOUNDARIES
        //-----------------------------------------------------------------------------

        //if the local frustrum right edge position goes past the bottom right vertex local position(most likely due to zooming). Then set the x position to 0
        float viewportLocal_BottomRightEdgePos = currentGameObjectLookingAt.transform.InverseTransformPoint(imitationCamera.ViewportToWorldPoint(new Vector3(1,0, zDistanceToFaceOfPoster))).x;
        float localPos_OfBottomRightVertex = currentGameObjectLookingAt.transform.InverseTransformPoint(worldSpaceVertex_BottomRight).x;
                                                                                //plus... that extra border
        if(viewportLocal_BottomRightEdgePos > localPos_OfBottomRightVertex + (1 * currentPosterViewSettings.extraBorder))
        {
        //SCRAPPED CAM MOVEMENT imitationCamera.transform.localPosition = new Vector3( 0f ,imitationCamera.transform.localPosition.y, imitationCamera.transform.localPosition.z);
        }

        //if the local frustrum right edge position goes past the bottom right vertex local position(most likely due to zooming). Then set the z position to 0
        float viewportLocal_TopLeftEdgePos = currentGameObjectLookingAt.transform.InverseTransformPoint(imitationCamera.ViewportToWorldPoint(new Vector3(0,1, zDistanceToFaceOfPoster))).z;
        float localPos_OfTopLeftVertex = currentGameObjectLookingAt.transform.InverseTransformPoint(worldSpaceVertex_TopLeft).z;
        if(viewportLocal_TopLeftEdgePos > localPos_OfTopLeftVertex + (1 * currentPosterViewSettings.extraBorder))
        {
        //SCRAPPED CAM MOVEMENT imitationCamera.transform.localPosition = new Vector3( imitationCamera.transform.localPosition.x , imitationCamera.transform.localPosition.y, 0);
        }
        
*/


        //INPUT CONTROLLS
        //----------------------------------


        //ZOOMING
        if(currentPosterViewSettings.canZoom)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
            {
                cameraNormalizedDistance -= 0.1f;
                float yPos = Mathf.Lerp(minYOffset, maxYOffset, cameraNormalizedDistance);
                imitationCamera.transform.localPosition = new Vector3(imitationCamera.transform.localPosition.x, yPos, imitationCamera.transform.localPosition.z);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backwards
            {
                cameraNormalizedDistance += 0.1f;
                float yPos = Mathf.Lerp(minYOffset, maxYOffset, cameraNormalizedDistance);
                imitationCamera.transform.localPosition = new Vector3(imitationCamera.transform.localPosition.x, yPos, imitationCamera.transform.localPosition.z);
            }
        }
        else
        {
            cameraNormalizedDistance = 1;
        }
        //clamp camera distance
        cameraNormalizedDistance = Mathf.Clamp(cameraNormalizedDistance, 0.6f, 1);


        //APPLY FORCED ZOOM RATIO
        float cameraNormalizedDistancePlusForcedOffset = cameraNormalizedDistance + currentPosterViewSettings.zoomForcedOffset;
        float yPosScaled = minYOffset + ((cameraNormalizedDistancePlusForcedOffset - 0.5f) * 2 * (maxYOffset - minYOffset));
        imitationCamera.transform.localPosition = new Vector3(imitationCamera.transform.localPosition.x, yPosScaled, imitationCamera.transform.localPosition.z);





        //IMPORTANT NOTE:
        //When debugging in scene view... make sure the imitation camera ENDS UP at the same position as the player camera... or else its not working right...


        //APPLY POSITION OFFSET TO ASSIGNED POSTER PLAYER IS LOOKING AT

        //switch the y and z or else the y will move foreward and backwards
        Vector3 switchedYandZValueOfLocalCameraPos = new Vector3(imitationCamera.transform.localPosition.x, imitationCamera.transform.localPosition.z, imitationCamera.transform.localPosition.y);
        cameraPosOffset = initialCameraPos - imitationCamera.transform.localPosition;

        Debug.Log("cameraPosOffset:" + cameraPosOffset);

        //warning!!! a lot of fuckery here... just remember. add the z instead of the y... (because the camera depth = y axis. and poster depth from player camera = typical z axis)


        currentGameObjectLookingAt.transform.SetParent(PlayerObjectInteractionStateMachine.Instance.playerCamera.transform);
        Quaternion desiredRotation2 = Quaternion.Euler(-90f, 0f, 0f);
        currentGameObjectLookingAt.transform.localRotation = desiredRotation2;

                                                                                                //FOR SOME REASON!!! if you dont add the cameraPosOffset y to the z axis of the poster local position... the depth will be fucked!
     //SCRAPPED CAM MOVEMENT   currentGameObjectLookingAt.transform.localPosition = initialPosterPos + new Vector3(cameraPosOffset.x, cameraPosOffset.z, -cameraPosOffset.y);
  
    

        //MAKES THE EMULATOR CAMERAS STAY IN THE EXACT SAME POSITION AS THE PLAYER CAMERA (or else there will be problems with multiple overlays)
        //what this does...
        //it posititions the poster in the same place as the player camera... plus the offset based on the child imitation camera of the poster...
        currentGameObjectLookingAt.transform.localPosition = new Vector3(0,0,imitationCamera.transform.localPosition.y);




/*

        //APPLY ROTATION OFFSET TO ASSIGNED POSTER PLAYER IS LOOKING AT
        //NOTE!!! the order of what things are multyplied are important! (imitationCamera.transform.localRotation * initialCameraRot)... (rotationDifference * initalPosterRot)... etc.
    
        Quaternion rotationDifference  = Quaternion.Inverse( imitationCamera.transform.localRotation) * initialCameraRot;
        // Now 'relativeRotation' contains the rotation offset between the two rotations
        Quaternion newRotation = rotationDifference * initalPosterRot;
        currentGameObjectLookingAt.transform.localRotation = newRotation;


        //get vector3 pos difference of imitation camera and player camera (they will be off, they need to be matched)
        Vector3 posDifBetweenImitationCamAndPlayerCam = (imitationCamera.transform.position - PlayerObjectInteractionStateMachine.Instance.playerCamera.transform.position);


        //add the offset (make sure to add it to the world position not the local!)
        currentGameObjectLookingAt.transform.position -= posDifBetweenImitationCamAndPlayerCam;

*/
    
    }

        //This is used to prevent the base rotation/position from being set every frame... it only gets set when the aspect ratio changes. (useful for videos that take a while to load)
        //Because if you update the base position/rotation every frame... the viewing mechanic will not work at all!
        private float lastFrameAspect;        

  void CalculateBaseViewingPosition()
        {
            //For calculating height/width of frame
            Vector3 worldSpaceVertex_TopLeft = currentGameObjectLookingAt.transform.TransformPoint(currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[3]);
            Vector3 worldSpaceVertex_BottomLeft = currentGameObjectLookingAt.transform.TransformPoint(currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[2]);
            Vector3 worldSpaceVertex_BottomRight = currentGameObjectLookingAt.transform.TransformPoint(currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[1]);

            float heightOfFrame = Vector3.Distance(worldSpaceVertex_BottomLeft, worldSpaceVertex_TopLeft);
            float widthOfFrame = Vector3.Distance(worldSpaceVertex_BottomLeft, worldSpaceVertex_BottomRight);

            float frustrumHeight = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * imitationCamera.fieldOfView);
            float frustrumWide = frustrumHeight * imitationCamera.aspect;
            
            float distance = 0;
            float frameAspect = widthOfFrame/heightOfFrame;
       
       
        if(currentPosterViewSettings.alignmentMode == "fit")
        {
            if(frameAspect >= imitationCamera.aspect)
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
            if(frameAspect >= imitationCamera.aspect)
            {
            distance = heightOfFrame / frustrumHeight;
            }
            else
            {
            distance = widthOfFrame / frustrumWide;
            }
        }


        distance += currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().distanceFromWall; // offset from object base
        
        
        imitationCamera.transform.SetParent(currentGameObjectLookingAt.transform);

        if(frameAspect != lastFrameAspect)            
        { 
        imitationCamera.transform.position = currentGameObjectLookingAt.transform.position - distance * -currentGameObjectLookingAt.transform.up;

        Debug.Log("imitationCamera.transform.position" + imitationCamera.transform.position);

                Debug.Log("imitationCamera.transform.localpos" + imitationCamera.transform.localPosition);

        //setting these euler angles makes the camera parented to the poster, face at the poster
        Quaternion desiredRotation = Quaternion.Euler(90f, 0f, 0f);
        imitationCamera.transform.localRotation = desiredRotation;


        maxYOffset = imitationCamera.transform.localPosition.y;
        Debug.Log("maxYoffset:" + maxYOffset);
        }
        lastFrameAspect = frameAspect;

        }





    
}
