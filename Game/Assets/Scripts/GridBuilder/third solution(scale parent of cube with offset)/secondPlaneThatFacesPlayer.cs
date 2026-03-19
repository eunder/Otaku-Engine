using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class secondPlaneThatFacesPlayer : MonoBehaviour
{
    public GridBuilderStateMachine gridBuilderStateMachine;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }
                 public Vector3 lookAtTarget;//the point in world space where the object looks at (not snapped)

    // Update is called once per frame
    void Update()
    {

                //    transform.LookAt(player.transform.position, Vector3.forward);





            if(gridBuilderStateMachine.wallHitnormal_start.y == 0)
            {
                    transform.rotation=Quaternion.Euler(new Vector3(-90,0,0));
            }
            else
            {
            // 90 degree snapper
             Vector3 aimingDir = player.transform.position - transform.position;
            float angle = -Mathf.Atan2(aimingDir.z, aimingDir.x) * Mathf.Rad2Deg + 90.0f;
            angle = Mathf.Round(angle / 90.0f) * 90.0f;
            Quaternion qTo = Quaternion.AngleAxis(angle, Vector3.up);
            transform.rotation = qTo;
            }



            // my bootleg solution
            /*
            if(Mathf.Abs(gridBuilderStateMachine.wallHitnormal.z) > 0)
            {
                transform.localEulerAngles = new Vector3(0.0f,90.0f,0.0f);
            }
            else
            {
                transform.localEulerAngles = new Vector3(0.0f,0.0f,0.0f);
            }
            */

    }
}
