using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PosterBillboard : MonoBehaviour
{
  public Transform mainPlayerCam;

    public bool useBillboard;
    public bool useCharacterBillboard;

    public Vector3 offset;
    
    void Start()
    {
      mainPlayerCam = GameObject.Find("MainPlayerCam").transform;
    }


    void Update()
    {
      if(useBillboard)
      {
        transform.rotation = Quaternion.LookRotation(transform.position - mainPlayerCam.position);
        transform.rotation *= Quaternion.Euler(-90, 0, 0); 
        
      }
      else if(useCharacterBillboard)
      {
        transform.rotation = Quaternion.LookRotation(transform.position - mainPlayerCam.transform.position);
        transform.rotation = Quaternion.Euler(-90f, transform.rotation.eulerAngles.y, 0);
      }
    }
}
