using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


//THIS RESTARTS THE ENTIRE LEVEL... PLAYER POSITION... EVERYTHING

public class ResetLevelManager : MonoBehaviour
{

    private static ResetLevelManager _instance;
    public static ResetLevelManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    public Vector3 startPos;

    public RawImage rawImageFadeToBlack;

    //NOTE, THIS IS MOSTLY USED FOR EVENTS "Do_Restart Level"! 
    public void ResetLevel()
    {

        //remove screen crush sfx
        MissionStatusManager.Instance.ResetValues();

        //restart all level changed parameters
        ResetLevelParametersManager.Instance.ResetAllLevelObjectsThatHaveChanged();



        //enable and position player at start position

        PlayerMovementBasic.Instance.transform.position = startPos + new Vector3(0.0f,0.768f,0.0f);
        PlayerMovementBasic.Instance.transform.rotation = Quaternion.Euler(0, 90, 0);

        PlayerMovementBasic.Instance.enabled = true;
        PlayerMovementBasic.Instance.transform.GetComponent<Rigidbody>().isKinematic = false;
        PlayerMovementBasic.Instance.rb.velocity = new Vector3(0,0,0);


        //reset mission status end event
        MissionStatusManager.Instance.missionEndEvent = false;

        //unfade from black... either normally fade in or do the elevator thing...
        rawImageFadeToBlack.DOFade(0, 1f);//.OnComplete(() => ResetLevelManager.Instance.ResetLevel());

    }



}
