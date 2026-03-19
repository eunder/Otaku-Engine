using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class PlayerMovementNoClip : MonoBehaviour
{

    private static PlayerMovementNoClip _instance;
    public static PlayerMovementNoClip Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }




  public float speedUp = 5.0f;
  private CharacterController charCont;

    public Transform playerCam;

    public Transform headtiltAngle_body;

  public Transform headtiltAngle;
  public Transform headtiltUnder;

  private Vector3 headtiltInitPosition;


  Transform T3;



  private float camRotZ = 0.0f;
  private float camRotX = 0.0f;
  public float camRotZ_MaxTilt = 30.0f;
  public float camRotX_MaxTilt = 5.0f;
  private float camRotZ_MaxTilt_Multiplier_Crouching = 1.0f;
  private float camRotZ_MaxTilt_Multiplier_TipToes = 1.0f;

  public float camtilitSpeed = 15.0f;

  private Vector3 move;

private float walkSpeed = 1.0f;
public float modifierSpeed = 1.0f;
      public float speed = 12.0f;
     public float x = 0.0f;
     public float z = 0.0f;
      float destx = 0.0f;
      float destz = 0.0f;
    RaycastHit m_Hit;

public float fallSpeed = 0.5f;

float heightYModifier = 0; //for going up and down in no clip (space and ctrl)
float heightYModifier_finalcalc = 0; //for going up and down in no clip (space and ctrl)
float heightYModifier_boost = 0; //little boost that happens when you enable noclip
float heightYModifier_speed = 0; //for being used on longer holds (to go faster)

public float heightYModifierAmount = 2; //for going up and down in no clip (space and ctrl)

public float gravity = 0;

public LayerMask layerMaskList;

public AudioSource noClipWind_AudioSource;

public float noClipWindPitch;

public PostProcessVolume volume;
ChromaticAberration chromaticAberration;

 public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue){
     
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
     
        return(NewValue);
    }

    void Start()
    {
            charCont = GetComponent<CharacterController>();
            headtiltInitPosition = headtiltAngle.localPosition;

            volume.profile.TryGetSettings(out chromaticAberration);

    }

    void OnDisable()
    {
      OnDisable_SFX();
    }


   public void OnEnable_SFX()
    {
       heightYModifier_boost += 1.0f;
    }

    public void OnDisable_SFX()
    {
      noClipWind_AudioSource.volume = 0;
      camRotZ = 0;
      camRotX = 0;
      heightYModifier_boost = 0;
    }

    public float movementAudioTrackCalc = 0; //for calcualting movement audio
    public float movementAudioTrack = 0; //for inputing audio into audio source volume

    void Update()
    {

      //to know how loud the noclip wind sound should be
      movementAudioTrackCalc = Mathf.Abs(z) + Mathf.Abs(x) + Mathf.Abs(heightYModifier);
      if(movementAudioTrackCalc > 1.0f)
      {
        movementAudioTrackCalc = 1.0f;
      }
      movementAudioTrack = Mathf.Lerp(movementAudioTrack, movementAudioTrackCalc, 5.0f * Time.deltaTime);

      noClipWind_AudioSource.volume = scale(0, 1f, 0.0f, 1.0f, Mathf.Abs(movementAudioTrack));

      //used to smoothly transition the pitch
      noClipWindPitch = Mathf.Lerp(noClipWindPitch, speed, 5.0f * Time.deltaTime);


      //modify the pitch depending on speed (possible speed = 0-10)
    //  noClipWind_AudioSource.pitch = scale(3, 10f, 0.2f, 0.6f, Mathf.Abs(noClipWindPitch));
      
      chromaticAberration.intensity.value = scale(0, 15f, 0.176f, 1.0f, Mathf.Abs(gravity));


 RaycastHit objectHitcrouch;
    Vector3 up = Vector3.up;
     Debug.DrawRay(gameObject.transform.position, up * 2.0f, Color.red);
     if (Physics.Raycast(gameObject.transform.position, up, out objectHitcrouch, 2.0f))
      {  
      }
      else
      {
        
      }

  float distancefromTopStand = Vector3.Distance(headtiltAngle.transform.localPosition, headtiltInitPosition);

 if(Input.GetKey(KeyCode.A))
{
 destx = -1.0f;
}
if(Input.GetKey(KeyCode.D))
{
 destx = 1.0f;
}
if((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) || (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)))
{
    destx = 0.0f;
}
    x = Mathf.Lerp(x, destx * walkSpeed * modifierSpeed, 5.0f * Time.deltaTime);



if(Input.GetKey(KeyCode.W))
{
 destz = 1.0f;
}
if(Input.GetKey(KeyCode.S))
{
 destz = -1.0f;
}
if((Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)) || (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)))
{
    destz = 0.0f;
}
    z = Mathf.Lerp(z, destz * walkSpeed * modifierSpeed, 5.0f * Time.deltaTime);

    //move = transform.right * x  + playerCam.forward * z; // ALT. for movement by camera foreward

    move = transform.right * x + transform.forward * z;
 
    //going up and down in no clip
   if(Input.GetKey(KeyCode.Space))
   {
    heightYModifier = 1 * heightYModifierAmount;
   }
   else if(Input.GetKey(KeyCode.LeftControl))
   {
    heightYModifier = -1 * heightYModifierAmount;
   }
   else
   {
    heightYModifier = 0;
   }

   heightYModifier_finalcalc = Mathf.Lerp(heightYModifier_finalcalc, heightYModifier, 5.0f * Time.deltaTime);


  //side to side tilt
  if(Input.GetKey(KeyCode.A))
  {
    camRotZ = Mathf.Lerp(camRotZ, camRotZ_MaxTilt * camRotZ_MaxTilt_Multiplier_Crouching * camRotZ_MaxTilt_Multiplier_TipToes, speedUp * Time.deltaTime);
  }
  else
  {
    camRotZ = Mathf.Lerp(camRotZ, 0.0f, speedUp * Time.deltaTime);
  }
    if(Input.GetKey(KeyCode.D))
  {
    camRotZ = Mathf.Lerp(camRotZ, camRotZ_MaxTilt * camRotZ_MaxTilt_Multiplier_Crouching * camRotZ_MaxTilt_Multiplier_TipToes * -1, speedUp * Time.deltaTime);
  }
  else
  {
    camRotZ = Mathf.Lerp(camRotZ, 0.0f, speedUp * Time.deltaTime);
  }

  //forward and backward tilt
    if(Input.GetKey(KeyCode.W))
  {
    camRotX = Mathf.Lerp(camRotX, camRotX_MaxTilt, speedUp * Time.deltaTime);
  }
  else
  {
    camRotX = Mathf.Lerp(camRotX, 0.0f, speedUp * Time.deltaTime);
  }
    if(Input.GetKey(KeyCode.S))
  {
    camRotX = Mathf.Lerp(camRotX, camRotX_MaxTilt * -1, speedUp * Time.deltaTime);
  }
  else
  {
    camRotX = Mathf.Lerp(camRotX, 0.0f, speedUp * Time.deltaTime);
  }

  //apply head tilt with lerp
    headtiltAngle_body.transform.localRotation = Quaternion.Slerp(headtiltAngle_body.transform.localRotation, Quaternion.Euler(new Vector3(camRotX,0.0f,camRotZ)), camtilitSpeed * Time.deltaTime);

   modifierSpeed = 1;
   gravity = 0;


    
    //the more you hold... the faster you go
    if(Mathf.Abs(destz) >= 1 || Mathf.Abs(destx) >= 1)
    {
        speed += Time.deltaTime;
    }
    else
    {
        speed = 5.0f;
    }

    //the more you hold... the faster you go
    if(Mathf.Abs(heightYModifier) >= 1)
    {
        heightYModifier_speed += Time.deltaTime;
    }
    else
    {
        heightYModifier_speed = 1.0f;
    }

    //to make the boost gradually get slower and prevent from going negative
    heightYModifier_boost -= Time.deltaTime;
    if(heightYModifier_boost < 0f)
    {
      heightYModifier_boost = 0f;
    }
        //y axis calc is complicated... three groups of things being added...
        transform.position = (transform.position + move * speed * Time.deltaTime) + new Vector3(0, (heightYModifier_speed * heightYModifier_finalcalc) + heightYModifier_boost,0) * Time.deltaTime;

        // charCont.Move(new Vector3(move.x * speed * modifierSpeed * Time.deltaTime, (move.y * speed * modifierSpeed * Time.deltaTime) + (heightYModifier_finalcalc * heightYModifier_speed * (speed/4) * Time.deltaTime ) + (heightYModifier_boost * Time.deltaTime), move.z * speed * modifierSpeed * Time.deltaTime));


 }

}
