using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformArrow_PlaneThatFacesPlayer : MonoBehaviour
{
    public GameObject player;
    public Transform arrow;
    // Update is called once per frame

    private Vector3 targetPoint;
    void Update()
    {
        if(arrow != null)
        {

            /*
                    // 90 degree snapper    THIS WORKS WITH YELLOW ARROW ONLY (X DIR)
             Vector3 aimingDir = player.transform.position - transform.position;
            float angle = -Mathf.Atan2(aimingDir.z, aimingDir.y) * Mathf.Rad2Deg + 5.0f;
            angle = Mathf.Round(angle / 5.0f) * 5.0f;
            Quaternion qTo = Quaternion.AngleAxis(angle, -Vector3.right);
            transform.localRotation = qTo;
            */

            /*
                    // 90 degree snapper  
             Vector3 aimingDir = player.transform.position - transform.position;
            float angle = -Mathf.Atan2(aimingDir.z, aimingDir.y) * Mathf.Rad2Deg + 5.0f;
            angle = Mathf.Round(angle / 5.0f) * 5.0f;
            Quaternion qTo = Quaternion.AngleAxis(angle, -Vector3.right);
            transform.localRotation = qTo;
            

            /*
              Vector3 relativePos = player.transform.position - transform.position;
 
              transform.LookAt(player.transform.position);

            var rotation = Quaternion.LookRotation(relativePos);
            rotation *= Quaternion.Euler(90, 0, 0); // t$$anonymous$$s adds a 90 degrees Y rotation
            transform.rotation = rotation;
         */


         /*
              Vector3 relativePos = player.transform.position - transform.position;

        transform.rotation = Quaternion.LookRotation(relativePos, Vector3.right);
*/


        /*
        if(Input.GetKeyDown(KeyCode.Tab))
        {
        Vector3 targetDir = player.transform.position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.up);
            Debug.Log(angle);
            gameObject.transform.localEulerAngles = new Vector3(angle, 0f, 0f);
         }
         */
    }
    }
}
