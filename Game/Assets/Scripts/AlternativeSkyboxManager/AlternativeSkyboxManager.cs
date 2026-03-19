using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternativeSkyboxManager : MonoBehaviour
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


        //set the poster layer to "overlay_skybox"
        storedLayer = currentGameObjectLookingAt.layer;
        currentGameObjectLookingAt.layer = 19;

        //if it is a stencil(has children)... set the layers on the children too
         foreach (Transform child in currentGameObjectLookingAt.transform)
         {
                 child.gameObject.layer = 19;
         }



        imitationCamera.transform.SetParent(currentGameObjectLookingAt.transform);
        imitationCamera.transform.localPosition = new Vector3(0,0,0);

        CalculateBaseViewingPosition();

        initialCameraPos = imitationCamera.transform.localPosition;
        initialCameraRot = imitationCamera.transform.localRotation;
        initialPosterPos = currentGameObjectLookingAt.transform.localPosition;
        initalPosterRot = currentGameObjectLookingAt.transform.localRotation;

        
        SetMaterialShaderProperties(renderQue);

        //if there are depth layers... sort them starting from 500... so that the order is respected
        for(int i = 0;  i < poster.GetComponent<PosterDepthLayerList>().posterDepthLayerList.Count; i++)
        {
            poster.GetComponent<PosterDepthLayerList>().posterDepthLayerList[i].GetComponent<Renderer>().sharedMaterial.renderQueue = 500 - i;
        }

        currentGameObjectLookingAt.GetComponent<GeneralObjectInfo>().DisableCollider();

        if(currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().isVideo)
        {
            currentGameObjectLookingAt.GetComponent<VLC_MediaPlayer>().PlayVideo(currentGameObjectLookingAt);
        }

        CreateSidePosters(poster);


        lastFrameAspect = 0; //so the positioning is reset
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


        GameObject leftPoster;
        GameObject rightPoster;
    public void CreateSidePosters(GameObject poster)
    {
        //NOTE!!! the drawing order for a posters vertices is 0 (top right), 1 (bottom right), 2(bottom left), 3(top left)            VERY IMPORTANT TO GET RIGHT!!!
        Vector3 worldSpaceVertex_TopLeft = currentGameObjectLookingAt.transform.TransformPoint(currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[3]);
        Vector3 worldSpaceVertex_BottomLeft = currentGameObjectLookingAt.transform.TransformPoint(currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[2]);
        Vector3 worldSpaceVertex_TopRight = currentGameObjectLookingAt.transform.TransformPoint(currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[0]);
        Vector3 worldSpaceVertex_BottomRight = currentGameObjectLookingAt.transform.TransformPoint(currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().vertices[1]);

        float width = Vector3.Distance(worldSpaceVertex_TopLeft, worldSpaceVertex_TopRight);
        width = width/distanceDepthMultiplier; //important!!! or else the posters will be way too far away...

        //create it first or else when you parent... that child will also get duplicated
         leftPoster = Instantiate(poster, poster.transform.position, Quaternion.identity);
         rightPoster = Instantiate(poster, poster.transform.position, Quaternion.identity);


        leftPoster.transform.SetParent(poster.transform);
        leftPoster.transform.localPosition = new Vector3(-width,0,0);
        leftPoster.transform.localRotation = Quaternion.identity;
        leftPoster.GetComponent<GeneralObjectInfo>().DisableCollider();

        //make sure to remove the things that arent needed on the poster anymore
        Destroy(leftPoster.GetComponentInChildren<Camera>().transform.gameObject);
        Destroy(leftPoster.GetComponentInChildren<MeshCollider>());
        Destroy(leftPoster.GetComponentInChildren<GeneralObjectInfo>());

        rightPoster.transform.SetParent(poster.transform);
        rightPoster.transform.localPosition = new Vector3(width,0,0);
        rightPoster.transform.localRotation = Quaternion.identity;
        rightPoster.GetComponent<GeneralObjectInfo>().DisableCollider();

        //make sure to destroy the extra created parented cameras\
        Destroy(rightPoster.GetComponentInChildren<Camera>().transform.gameObject);
        Destroy(rightPoster.GetComponentInChildren<MeshCollider>());
        Destroy(rightPoster.GetComponentInChildren<GeneralObjectInfo>());



        //assign the appripriate sort order
        for(int i = 0;  i < leftPoster.GetComponent<PosterDepthLayerList>().posterDepthLayerList.Count; i++)
        {
            leftPoster.GetComponent<PosterDepthLayerList>().posterDepthLayerList[i].GetComponent<Renderer>().sharedMaterial.renderQueue = 500 - i;
        }
        for(int i = 0;  i < rightPoster.GetComponent<PosterDepthLayerList>().posterDepthLayerList.Count; i++)
        {
            rightPoster.GetComponent<PosterDepthLayerList>().posterDepthLayerList[i].GetComponent<Renderer>().sharedMaterial.renderQueue = 500 - i;
        }


        
        //for giving a unique id to the duplicated depth posters(if there are any)
        PosterDepthLayerStencilRefManager.Instance.AssignCorrectStencilRefsToAllPostersInScene();

    }


    public void RemoveCameraEmulator()
    {
        //set the layer of the poster back
        currentGameObjectLookingAt.layer = storedLayer;

        //if it is a stencil(has children)... set the layers on the children too
         foreach (Transform child in currentGameObjectLookingAt.transform)
         {
            child.gameObject.layer = storedLayer;
         }

        //set render que of the poster back to 3000 (the standard for the game)
        //currentGameObjectLookingAt.GetComponent<Renderer>().material.renderQueue = 3000;


        currentGameObjectLookingAt.GetComponent<GeneralObjectInfo>().ResetVisibility();


        //destroy left and right posters
        if(leftPoster != null)
        {
        leftPoster.GetComponent<MeshRenderer>().sharedMaterial = null;
        Destroy(leftPoster);
        }
        if(rightPoster != null)
        {
        rightPoster.GetComponent<MeshRenderer>().sharedMaterial = null;
        Destroy(rightPoster);
        }

        Destroy(imitationCamera.gameObject);
        Destroy(gameObject);
    }


        float currentXVal = 0.5f ; //centered
        float currentYVal = 0.5f ;  //centered

    // Update is called once per frame
    void Update()
    {
        //ran every frame! a bit unoptimized but its a duct tape quick fix for the problem of the base viewing position logic running before the video even started
        //BUG: this causes problems when used with the skybox emulator. fix later!
        CalculateBaseViewingPosition();

        //update these or else the side posters wont update with correct aspect ratios! (this is only used in the skybox emulator poster viewing class)
        rightPoster.GetComponent<PosterMeshCreator>().image = currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().image;
        leftPoster.GetComponent<PosterMeshCreator>().image = currentGameObjectLookingAt.GetComponent<PosterMeshCreator>().image;



        rightPoster.GetComponent<MeshRenderer>().sharedMaterial = currentGameObjectLookingAt.GetComponent<GeneralObjectInfo>().baseMatList[0];
        leftPoster.GetComponent<MeshRenderer>().sharedMaterial = currentGameObjectLookingAt.GetComponent<GeneralObjectInfo>().baseMatList[0];

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

        float xmin = left_edge_pos;   //SKYBOX_MODE NOTE: HAD TO GET RID OF THE EXTRAS IN ORDER TO ALLOW IT TO GO ALL THE WAY TOWARDS THE EDGES... OR ELSE SEAMLESS SNAPPING WONT WORK!
        float xmax = Mathf.Abs(xmin);

        float zmin = bottom_edge_pos + (camHeight/2)  - (1 * currentPosterViewSettings.extraBorder); 
        
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

            
        //convert up/down to normalized value
        float minPitch = -90f;
        float maxPitch = 90f;

        float pitch = imitationCamera.transform.eulerAngles.x;

        if (pitch > 180f)   {pitch -= 360f;}


        //convert to 0 to 1
        pitch = Mathf.InverseLerp(minPitch, maxPitch, pitch);
        pitch = Mathf.Abs(pitch - 1);



        //convert left/right to normalized value
        float yaw = imitationCamera.transform.eulerAngles.y;

        // Normalize the yaw to a range of 0 to 1.
        float yawNormal = yaw / 360f;

                

        if(currentPosterViewSettings.inverseLook)
        {
            pitch = Mathf.Abs(pitch - 1);
        //    yawNormal = Mathf.Abs(yawNormal - 1);
        }

        imitationCamera.transform.localPosition = new Vector3( Mathf.Lerp(xmin, xmax, yawNormal),imitationCamera.transform.localPosition.y,  Mathf.Lerp(zmin, zmax, pitch));
                





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
        imitationCamera.transform.localRotation = desiredRotation;

        //Add the rotation offset
        imitationCamera.transform.localRotation *= Quaternion.Euler(imageViewing_rotationOffset);





        //SKYBOX MOD NOTE: THIS WILL FUCK UP THE SEAMLESS SCROLLING EFFECT!

        /*
        //CENTERING OF CAMERA DUE TO CAMERA FRUSTRUM GOING PAST CERTAIN BOUNDARIES
        //-----------------------------------------------------------------------------

        //if the local frustrum right edge position goes past the bottom right vertex local position(most likely due to zooming). Then set the x position to 0
        float viewportLocal_BottomRightEdgePos = currentGameObjectLookingAt.transform.InverseTransformPoint(imitationCamera.ViewportToWorldPoint(new Vector3(1,0, zDistanceToFaceOfPoster))).x;
        float localPos_OfBottomRightVertex = currentGameObjectLookingAt.transform.InverseTransformPoint(worldSpaceVertex_BottomRight).x;
                                                                                //plus... that extra border
        if(viewportLocal_BottomRightEdgePos > localPos_OfBottomRightVertex + (1 * currentPosterViewSettings.extraBorder))
        {
        imitationCamera.transform.localPosition = new Vector3( 0f ,imitationCamera.transform.localPosition.y, imitationCamera.transform.localPosition.z);
        }

        //if the local frustrum right edge position goes past the bottom right vertex local position(most likely due to zooming). Then set the z position to 0
        float viewportLocal_TopLeftEdgePos = currentGameObjectLookingAt.transform.InverseTransformPoint(imitationCamera.ViewportToWorldPoint(new Vector3(0,1, zDistanceToFaceOfPoster))).z;
        float localPos_OfTopLeftVertex = currentGameObjectLookingAt.transform.InverseTransformPoint(worldSpaceVertex_TopLeft).z;
        if(viewportLocal_TopLeftEdgePos > localPos_OfTopLeftVertex + (1 * currentPosterViewSettings.extraBorder))
        {
        imitationCamera.transform.localPosition = new Vector3( imitationCamera.transform.localPosition.x , imitationCamera.transform.localPosition.y, 0);
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

                                                                                                //FOR SOME REASON!!! if you dont add the cameraPosOffset y to the z axis of the poster local position... the depth will be fucked!
        currentGameObjectLookingAt.transform.localPosition = initialPosterPos + new Vector3(cameraPosOffset.x, cameraPosOffset.z, -cameraPosOffset.y);
  
    

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


    
    }


    public float distanceDepthMultiplier = 50f; //for making it look good in vr
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
            frameAspect = Mathf.Round(frameAspect * 100f) / 100f; //round to prevent floating point problems

       
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
            

                    if(frameAspect != lastFrameAspect)            
        {
        imitationCamera.transform.position = currentGameObjectLookingAt.transform.position - distance * -currentGameObjectLookingAt.transform.up;

        //add that distance so that it looks good in vr mode (and renders behind everything)
        currentGameObjectLookingAt.transform.localPosition =  currentGameObjectLookingAt.transform.localPosition + new Vector3(0,0, distanceDepthMultiplier);
        currentGameObjectLookingAt.transform.localScale = new Vector3(1,1,1) * distanceDepthMultiplier;


        imitationCamera.transform.SetParent(currentGameObjectLookingAt.transform);

        //setting these euler angles makes the camera parented to the poster, face at the poster
        Quaternion desiredRotation = Quaternion.Euler(90f, 0f, 0f);
        imitationCamera.transform.localRotation = desiredRotation;


        maxYOffset = imitationCamera.transform.localPosition.y;
        }
                lastFrameAspect = frameAspect;

        }





    
}
