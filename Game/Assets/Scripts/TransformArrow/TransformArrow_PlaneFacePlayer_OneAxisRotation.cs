using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformArrow_PlaneFacePlayer_OneAxisRotation : MonoBehaviour
{
    //NOTE: players have to use the z axis to determine the value...

    public Transform playerCam;
    public Transform transformUp;


    // Update is called once per frame
    void Update()
    {
        UpdateOrientation();
    }

    public void UpdateOrientation() //this is also called right before setting the start offset off the arrow/widget (if you dont call it there, it wont update on time)
    {
        if(transformUp != null)
        {
            // Calculate the direction from "ObjectA" to the player
            Vector3 directionToPlayer = playerCam.position - transform.position;

            // Project the direction onto "ObjectA"'s local up vector
            Vector3 projectedDirection = Vector3.ProjectOnPlane(directionToPlayer, transformUp.up).normalized;

            // Calculate the target rotation to directly face the player
            Quaternion targetRotation = Quaternion.LookRotation(projectedDirection, transformUp.up);

            // Apply the additional rotation around the local X-axis
            targetRotation *= Quaternion.Euler(90f, 0f, 0f); // Change the 90f to the desired angle you want to add

            // Apply the rotation to "ObjectA"
            transform.rotation = targetRotation;
        }
    }

}
