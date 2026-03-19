using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;
using DG.Tweening;

    //NOTE: this class and the other similar classes handle all the path events in this "emulator" way... it applies the offsets to the "referenceObject" every frame 


public class PathEmulator : MonoBehaviour
{

    public GameObject referenceObject;

    Vector3 last_pos;
    Quaternion last_rot;
    

    float currentPathDeltaPos;
    float lastPathDeltaPos;


    public bool additive = true;
    public bool rotationOnly = true;
    public bool moveToPath = false;
    public bool reverse = false;


    void Start()
    {

        if(referenceObject.GetComponent<PathEmulatorList>() == null)
        {
            referenceObject.AddComponent<PathEmulatorList>();
        }

        referenceObject.GetComponent<PathEmulatorList>().pathEmulators.Add(gameObject);


        //special case only used for when a path is: reversed, additive, and rotation only...
        //if you dont do this... then for some reason playing the additive_reverse_rotationOnly event will make the object incorrectly snap right at the start...
        if(reverse && additive && rotationOnly)
        {
        transform.rotation = GetComponent<splineMove>().pathContainer.waypoints[GetComponent<splineMove>().pathContainer.waypoints.Length - 1].rotation;
        }

        if(additive == false)
        {
            if(moveToPath)
            {

            }
            else
            {
                referenceObject.transform.position = GetComponent<splineMove>().pathContainer.transform.position;

                if(GetComponent<splineMove>().waypointRotation == splineMove.RotationType.all)
                referenceObject.transform.rotation = GetComponent<splineMove>().pathContainer.transform.rotation;
            }


        }

        //if it is set to play on reverse AND its not additive... then set the starting point of the reference object to the end point position
        if(reverse && additive == false)
        {
            referenceObject.transform.position = GetComponent<splineMove>().pathContainer.waypoints[GetComponent<splineMove>().pathContainer.waypoints.Length - 1].position;
            
            if(GetComponent<splineMove>().waypointRotation == splineMove.RotationType.all)
            referenceObject.transform.rotation = GetComponent<splineMove>().pathContainer.waypoints[GetComponent<splineMove>().pathContainer.waypoints.Length - 1].rotation;
        }


        // if you dont have this check... then when the thing starts... there will either be problems with (additive OR non-additive loops) having more frame skips solves the issue for additive stuff...
        if(additive)
        {
            frameSkip = 3;
        }
        else
        {
            frameSkip = 1;
        }







        last_pos = transform.position;
        last_rot = transform.rotation;
    }


    Quaternion rotationDifference;
    int frameSkip = 0; 
    //YOU HAVE TO DO THIS BECAUSE SIMPLE WAYPOINT SYSTEM UPDATES THE WAYPOINT ROTATION ONE FRAME TOO LATE!!!
    //YOU DELAY THE UPDATING OF THE ROTATION BY 2 FRAMES...
    //just try it... it unity pause mode, with a path thats looping with waypoint rotation... notice how when the object goes back to the first node/waypoint... the rotation is still as if it were on the last one...



    //NOTE: multiple additive rotation events should be added correctly... however... endless looping rotation events will always be innacurate...

void Update()
{   
        if(GetComponent<splineMove>().tween != null)
        currentPathDeltaPos = GetComponent<splineMove>().tween.ElapsedPercentage();

        //reset values
        if(currentPathDeltaPos < lastPathDeltaPos)
        {
            // if you dont have this check... then when the thing loops... there will either be problems with (additive OR non-additive loops) having more frame skips solves the issue for additive looping stuff...
            if(additive)
            {
                frameSkip = 3;
            }
            else
            {
                frameSkip = 1;
            }

         //   Debug.Log("Restarted!");

            if(additive == false)
            {
                if(GetComponent<splineMove>().waypointRotation == splineMove.RotationType.all)
                referenceObject.transform.rotation = GetComponent<splineMove>().pathContainer.waypoints[0].rotation;
            }

        }


        //HORRIBLE WAY TO DO IT BUT YOU HAVE TO BECAUSE (see comment about frameSkip above)

        if(rotationOnly != true)
        {
            //position difference
            if(frameSkip <= 1)
            {
            Vector3 positionDifference = transform.position - last_pos;
            if(reverse)
            {
          //  positionDifference = positionDifference * -1;
            }
            referenceObject.transform.position += positionDifference;
            }
        }


        //only use the frame skip if there is looping... after all, the WHOLE POINT OF FRAME SKIPPING MECHANIC IS FOR LOOPING ISSUES AND ROTATION!
        if(GetComponent<splineMove>().loopType == splineMove.LoopType.none)
        {
                rotationDifference = transform.rotation * Quaternion.Inverse(last_rot); 

                if(GetComponent<splineMove>().waypointRotation == splineMove.RotationType.all)
                referenceObject.transform.rotation = rotationDifference * referenceObject.transform.rotation;
        }
        else
        {
            //if there is looping... use the frame skip bullshit
            if(frameSkip == 0)
            {
                rotationDifference = transform.rotation * Quaternion.Inverse(last_rot); 

                if(GetComponent<splineMove>().waypointRotation == splineMove.RotationType.all)
                referenceObject.transform.rotation = rotationDifference * referenceObject.transform.rotation;
            }
        }
        


        frameSkip--;
        if(frameSkip < 0)
        {
            frameSkip = 0;
        }
   



        last_pos = transform.position;
        last_rot = transform.rotation;

        lastPathDeltaPos = currentPathDeltaPos;

    }


    
}
