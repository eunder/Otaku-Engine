using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoardFaceCamera : MonoBehaviour
{

    public Camera mainPlayerCam;

    public bool useStaticBillBoard;

    // Update is called once per frame
    void Update()
    {

        if(!useStaticBillBoard)
        {
        transform.rotation = Quaternion.LookRotation(transform.position - mainPlayerCam.transform.position);
        }
        else
        {
        transform.rotation = mainPlayerCam.transform.rotation;
        }

        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
