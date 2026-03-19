using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

public class PlayerMovementBasic : MonoBehaviour
{

    private static PlayerMovementBasic _instance;
    public static PlayerMovementBasic Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }




    public Vector3 velocity = new Vector3(0,0,0);
     Vector3 previousVelocity; //used mostly for making sure the proper values get inputed for land effects
    public float speed = 0;
    public Rigidbody rb;

    public Vector3 gravity_Standing;
    public Vector3 gravity_Crouched;

    public PhysicMaterial physicsMaterial_Standing;
    public PhysicMaterial physicsMaterial_Crouched;

    public float raycastGroundCheckDistance = 0.1f;
    public Transform playerCamera;
    public float rotationLerpSpeed = 1.0f;


    public bool isGrounded = false;
    public float timeOnGround = 0.0f;
    public bool isAllowedToBeGroundedAgain_mod = true; //used to prevent the player from sticking to the ground when jumping while in running state
    public bool isRunning =  false;
    public float jumpForce = 1.0f;
    public float slopeSteepness;

    public float maxSlopeAngleUntilPlayerSlides;
    public float maxSlopeAngleUntilPlayerSlides_Standing;
    public float maxSlopeAngleUntilPlayerSlides_Crouched; //NOTE!!! due to a logic conflict where player is "ungrounded" sliding a a slope crouched... they wont be able to stand up... just keep this the same as "_standing" variable for now...

    public float maxSpeedChange = 1f;
    public float control;

    public AnimationCurve speedControl; //used to determine the control player has while wall running
    public AnimationCurve speedControl_BasedOnAccumilatedSpeed; //used to determine the control player has while wall running (using accumilated speed instead)
    public AnimationCurve speedControl_air; //used to determine the control player has while in the air


    public bool enableLessFrictionOnNoInput;
    public AnimationCurve speedControl_playerInput; //used to determine the control player has based on input (0 - 1)

    float cameraStartLocalYPos;

    public float speedToWallRun = 5.0f;

    public float accumilatedSpeed = 0;
    public float accumilatedSpeed_influence;
    public float accumilatedSpeed_influence_air;
    public float accumilatedSpeed_max = 10f;
    public float accumilationSpeed = 0.1f; //how fast the player should accumilate
    public float loseSpeed = 0.1f; //how fast the player should loss
    public float loseSpeed_flatGround = 0.1f; //how fast the player should loss


    //to tell if player is going up/down hill
    private Vector3 previousPosition;
    public bool movingUphill;



    //SPEED LINE MECHANIC
    public GameObject speedLines_Particle_GameObject;
    public float speedLines_Particle_GameObject_offset;
    public AnimationCurve speedLines_ParticleTransparency_AnimationCurve;

    public ParticleSystem speedLines_ParticleSystem;
    public Material speedLines_Material;



    //CAMERA ROTATION

    public Transform headtiltAngle_body;
    public Transform headtiltAngle;
    private Vector3 headtiltInitPosition;

    private float camRotZ = 0.0f;
    private float camRotX = 0.0f;
    public float camRotZ_MaxTilt = 0f;
    public float camRotX_MaxTilt = 0f;

    private float camRotZ_MaxTilt_Multiplier_Crouching = 1.0f;

    public float camTiltSpeedUp = 5.0f;
    public float camtilitSpeed = 15.0f;





    void Start()
    {
        cameraStartLocalYPos = playerCamera.transform.localPosition.y;
        volume.profile.TryGetSettings(out chromaticAberration);




        //correct the player crush detection capsule on start
        crushDetectionCapsule.capsuleCollider.height = 1.6f * crushDetectionCapsule.shrinkMultiplier;
        crushDetectionCapsule.capsuleCollider.center = new Vector3(0,0,0);
    }

    //note: make sure to freeze the rigidbody contraints and NOT do kinematic stuff... because enabling/disavling kinematic bool triggers OnEnter events in the unity physics engine...
   void OnDisable()
    {
      RemovePlayerFromPlatform();

      fastWind_audioSource.volume = 0;
      rb.constraints = RigidbodyConstraints.FreezeAll;
      speedLines_Particle_GameObject.SetActive(false);
    }

    void OnEnable()
    {
        if(SaveAndLoadLevel.Instance.isLevelLoaded == true && PlayerMovementTypeKeySwitcher.Instance.isInFlyMode == false)
        rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        speedLines_Particle_GameObject.SetActive(true);
    }



    //PARENTED MODIFIER MECHANIC
    public GameObject parentedMechanic_currentParentBlock;



    //MOVING PLATFORM MECHANIC
    //Note: The game sets the current platform to null when... 1. the player is not grounded.   2. when the player detects a standable block but with no rigidbody attached
    //Note: The game adds the current platform when... player is standing on a block with a rigidbody(meaning its moving)

    public GameObject platformMechanic_currentPlatform;

    public Vector3 platformMechanic_rigidBody_lastPosition;
    public Vector3 platformMechanic_playerVelocity;

    public void RemovePlayerFromPlatform()
    {   
        if(platformMechanic_currentPlatform)
        {
                    platformMechanic_currentPlatform = null;

                    //only remove player if there is not parent modifier
                    if(parentedMechanic_currentParentBlock)
                    {
                        transform.SetParent(parentedMechanic_currentParentBlock.transform);
                    }
                    else
                    {
                    transform.SetParent(null, true); //"true" second argument so that the player scale is respected
                    transform.localScale = new Vector3(1,1,1); //just in case...
                    //make sure the player velocity is continue'd
                        //rb.velocity = platformMechanic_playerVelocity; //didnt need it anymore... for some reason... 
                    }

        }
    }

    public void AttachPlayerToPlatform(GameObject platform)
    {
                    platformMechanic_currentPlatform = platform;

                    //only attach player if there is no parent modifier
                    if(parentedMechanic_currentParentBlock)
                    {
                        transform.SetParent(parentedMechanic_currentParentBlock.transform);
                    }
                    else
                    {
                        transform.SetParent(platform.transform);
                    }
                

                    // this is here just to make sure the distance travelled gets properly calulated... without this... it would still calculate the last position of the previous platform(which may be far away and cause a "snap" to happen)
                    // BUG!!! sometimes when walking on a moving platform... the player might sutter and hitch-up... this is due to the player being attached to a new platform when they are standing right inbetween two blocks(which are part of the same platform). i already tried to debug this using rigidbody contact points but it seems like the issue lies with the player being parented again AFTER they have already been switched... resulting in the debug.log bellow being called every very frequently!!! 

                    lastPosition_x = transform.localPosition.x;
                    lastPosition_z = transform.localPosition.z;
                    
                    Debug.Log("ON NEW PLATFORM!" + platformMechanic_currentPlatform.name);
    }
    void Update()
    {  
        //CHECK IF PLAYER IS STRAFING                           //remove the "&& isGrounded == false" part if you want that mechanic that maintains player speed forever while grinding on the ground... 
        if ((verticalInput == 0 && horizontalInput != 0) && isGrounded == false)
        {
            isStrafing = true;
        }
        else
        {
            isStrafing = false;
        }


        //to calculate velocity for when player jumps off a platform
        if(isGrounded)
        {
            platformMechanic_playerVelocity = (transform.position - platformMechanic_rigidBody_lastPosition) / Time.deltaTime;
            platformMechanic_rigidBody_lastPosition = transform.position;
        }
                GetInputValues();


                CalculateSlopeSteepness();

                //double jump
                /*
                if(isGrounded == false  && Input.GetKeyDown(KeyCode.Space))
                {   

                    timeOnGround += Time.deltaTime;
                    quedForJump = true;

                        //move player depending on surface normal (and player foreward)
                        Vector3 forwardForce = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized; //make sure to normalize or else the player will go slower up/down hill
                        Vector3 rightForce = Vector3.ProjectOnPlane(transform.right, hit.normal).normalized;

                        desiredVelocity = (movement.z * forwardForce + movement.x * rightForce) * rb.velocity.magnitude; //multiply by magnitude to send the player in a direction with momentum



                    wallJump_ComboCount += 1;

                    //wallJump_audioSource.pitch = scale(0,20, 0.75f, 1.4f,gravity);
                    wallJump_audioSource.PlayOneShot(wallJump_audioClip, 1.0f);

                    rb.velocity = new Vector3(desiredVelocity.x,jumpForce,desiredVelocity.z);


                }
                else
                {
                    timeOnGround = 0.0f;
                }


                */


               

                if(isGrounded)
                {
                    canUncrouch = true;
                    timeOnGround += Time.deltaTime;
                }
                else
                {
                    timeOnGround = 0.0f;
                }




        //HEAD TILT MECHANIC

        //side to side tilt
        if(Input.GetKey(KeyCode.A))
        {
            camRotZ = Mathf.Lerp(camRotZ, camRotZ_MaxTilt * camRotZ_MaxTilt_Multiplier_Crouching, camTiltSpeedUp * Time.deltaTime);
        }
        else
        {
            camRotZ = Mathf.Lerp(camRotZ, 0.0f, camTiltSpeedUp * Time.deltaTime);
        }
            if(Input.GetKey(KeyCode.D))
        {
            camRotZ = Mathf.Lerp(camRotZ, camRotZ_MaxTilt * camRotZ_MaxTilt_Multiplier_Crouching  * -1, camTiltSpeedUp * Time.deltaTime);
        }
        else
        {
            camRotZ = Mathf.Lerp(camRotZ, 0.0f, camTiltSpeedUp * Time.deltaTime);
        }

        //forward and backward tilt
            if(Input.GetKey(KeyCode.W))
        {
            camRotX = Mathf.Lerp(camRotX, camRotX_MaxTilt, camTiltSpeedUp * Time.deltaTime);
        }
        else
        {
            camRotX = Mathf.Lerp(camRotX, 0.0f, camTiltSpeedUp * Time.deltaTime);
        }
            if(Input.GetKey(KeyCode.S))
        {
            camRotX = Mathf.Lerp(camRotX, camRotX_MaxTilt * -1, camTiltSpeedUp * Time.deltaTime);
        }
        else
        {
            camRotX = Mathf.Lerp(camRotX, 0.0f, camTiltSpeedUp * Time.deltaTime);
        }

            
        //apply head tilt with lerp
        headtiltAngle_body.transform.localRotation = Quaternion.Slerp(headtiltAngle_body.transform.localRotation, Quaternion.Euler(new Vector3(camRotX,0.0f,camRotZ)), camtilitSpeed * Time.deltaTime);






/*

                //CLING MECHANIC
                    DrawLedgeGrabCast();

                 //teleport on click
                if(Input.GetMouseButton(1))
                {
                    UnHideTargeters();

                if(Input.GetMouseButtonDown(0) && canTeleport && canLedgeGrabOrTeleport)
                {
                    if (cling_blueBoxShapeCast_hitDetect)
                    {
 
                        canLedgeGrabOrTeleport = false;
                        Vector3 destination = SphereOrCapsuleCastCenterOnCollision(SimpleSmoothMouseLook.Instance.transform.position + SimpleSmoothMouseLook.Instance.transform.forward * boxTeleportCast_startOffset, SimpleSmoothMouseLook.Instance.transform.forward, cling_shapeHit.distance);
                        PositionPlayerOnCastEvent(destination);

                        UnHideLineRenderersAndCling();
                    }
                }
                }
                else
                {
                    HideTargeters();
                }

*/


                //LEDGE GRAB MECHANIC


                /*
                float teleportHitFloorSlopeAngle = Vector3.Angle(Vector3.up, cling_shapeHit.normal);
                DrawLedgeGrabCast();

                //space to ledge climb (a little wonky... there is a problem with the box cast going past the block when up-close)
                if(Input.GetKeyDown(KeyCode.Space) && cling_blueBoxShapeCast_hitDetect && cling_shapeHit.distance < ledgeGrab_distance && teleportHitFloorSlopeAngle > minimumAngleWhereUserGrabsOntoFloor && canLedgeGrabOrTeleport && cling_isThereABlockInTheWayOfLedgeClimbing == false)
                {
                            if(cling_hit_downPositionFound && cling_boxHits.Length <= 0)
                    {
                     rb.isKinematic = true;

                    canLedgeGrabOrTeleport = false;
                    ledgeGrag_AudioSource.Play();

                    //turn off crush detection capsule for duration of climb event
                    crushDetectionCapsule.gameObject.SetActive(false);



                    cling_moveTween = transform.DOMove(cling_hit_downPosition.point + (cling_shapeHit.normal * ledgeBoxPlacementDepth) + new Vector3(0,boxTeleportCollider.size.y/2,0), LedgeGragDoMoveSpeed).OnComplete(() => OnEndDestReached()); 
                    }
                }
*/




                                            //ONLY enable when falling... AND when holding space... or else it will make other things jank
                    if(((rb.velocity.y < -0.5f) && holdingJump)) //doing it this way instead of "isStrafing" prevents a terrible bug when walking sideways on and off of ramps/corners...
                    {
                        boxColliderThatStaysInPlaceForBhopping.SetActive(true);
                    }
                    else
                    {
                        boxColliderThatStaysInPlaceForBhopping.SetActive(false);
                    }











                //WALL JUMPING MECHANIC
                  //PRESS SPACE TO WALL JUMP


                if(isGrounded )
                {
                  blockAboutToBeUsed = null;
                  blockUsedToWallJump = null;
               
                  canJumpAgain = false;
                }

                        //wall jump                                                                 OR... canJumpAgain set to true
                if((canWallJump && isGrounded == false && blockUsedToWallJump != blockAboutToBeUsed) || canJumpAgain)
                {
                if(rb.isKinematic == false) //this is purely done to prevent a walljump from happening while the player is ledge climbinb
                {
                    if(Input.GetKeyDown(KeyCode.Space))
                    {
                        canJumpAgain = false;

                        //move player depending on surface normal (and player foreward)
                        Vector3 forwardForce = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized; //make sure to normalize or else the player will go slower up/down hill
                        Vector3 rightForce = Vector3.ProjectOnPlane(transform.right, hit.normal).normalized;

                        desiredVelocity = (movement.z * forwardForce + movement.x * rightForce) * wallJumpAmount; //multiply by magnitude to send the player in a direction with momentum



                    wallJump_ComboCount += 1;

                    //wallJump_audioSource.pitch = scale(0,20, 0.75f, 1.4f,gravity);
                    wallJump_audioSource.PlayOneShot(wallJump_audioClip, 1.0f);

                    blockUsedToWallJump = blockAboutToBeUsed;

                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                    rb.velocity += new Vector3(desiredVelocity.x,jumpForce,desiredVelocity.z);

                    accumilatedSpeed = new Vector3(0,jumpForce,0).magnitude;

                    //gravity += 5f * (1/(float)wallJump_ComboCount);
                    //isGrounded = false;
                    canWallJump = false;
                    }
                }
                 }

             




                //SPEED LINE MECHANIC

                speedLines_Particle_GameObject.transform.position = playerCamera.transform.position + rb.velocity.normalized * speedLines_Particle_GameObject_offset;
                speedLines_Particle_GameObject.transform.LookAt(playerCamera.transform);

                speedLines_Material.color = new Color(speedLines_Material.color.r, speedLines_Material.color.g, speedLines_Material.color.b, speedLines_ParticleTransparency_AnimationCurve.Evaluate(speed));




    }

    public Vector3 addedForce;

    RaycastHit hit;

	void ClearState () {
		isGrounded = false;
        floorCollisionCount = 0;
 	}

    public float currentHittingRaySlopeAngle;
    public float lastHittingSlopeAngle; //used to store the previous slope angle in order to determine if the change of angle was too sharp


    //GRINDING MECHANIC (when the player turns sharply )

    public float grinding_AngleThreshold = 5.0f; // Set an angle threshold for when the sound should play
    float grinding_checkInterval_timer;
    public float grinding_checkInterval;
    public AudioSource grinding_playGrind_AudioSource;
    public AudioClip grinding_playGrind_AudioClip;
    public GameObject grinding_SmokeParticle_Prefab;

    public float grinding_maxPlayerVelocityUntilTriggerGrind = 1;
    public float grinding_accumilatedSpeedLoseOnGrind = 5;

    void CheckForSharpTurn()
    {
        // Calculate the angle between the desiredVelocity and current velocity
        float angle = Vector3.Angle(rb.velocity, desiredVelocity);

        if(isGrounded)
        {                                       //this makes sure the grind does not happen when the player is simply pushing against a wall
        if (angle > grinding_AngleThreshold && rb.velocity.magnitude > grinding_maxPlayerVelocityUntilTriggerGrind)
        {
            // Play your grinding sound here
            // For example, if you have an AudioSource attached to the same GameObject:
            // GetComponent<AudioSource>().Play();

                                                                // positiong: position of capsul + (bottom of capsul + forward of capsule))
            Instantiate(grinding_SmokeParticle_Prefab, transform.position + -transform.up * 1f + transform.forward * 1, Quaternion.LookRotation(Vector3.up, transform.up));
            grinding_playGrind_AudioSource.PlayOneShot(grinding_playGrind_AudioClip, 1.0f);

            //remove both types of player speeds on grind...
            accumilatedSpeed -= grinding_accumilatedSpeedLoseOnGrind;
            rb.velocity = rb.velocity * (1f - grinding_accumilatedSpeedLoseOnGrind * Time.fixedDeltaTime);
        }
        }

    }



    //CROUCHING MECHANIC
    public bool isCrouching = false;
    public bool canUncrouch = false; //mostly used for making sure the player cant spam crouch in the air

    public float crouch_yPosOffset = 0;  //to add to the camera finaly y pos
    public float crouch_yPosOffsetCamera = 0;

    public CapsuleCollider canStand_Collider;
    public CapsuleCollider canUncrouchWhileInAir_Collider; //to make sure player dosnt uncrouch right before hitting the floor. not... this collider ALSO takes not of the stuff above the player... for example... so that they dont get crushed after coming out of a vent right with a ceiling right above them.
    public int canUncrouchColHitCount = 0;
    public LayerMask obstacleMask; // Define the layers that represent obstacles
    bool CanStandUp()
    {
        Vector3 p1 = transform.position + canStand_Collider.center + Vector3.up * -canStand_Collider.height * 0.3f;
        Vector3 p2 = transform.position + canStand_Collider.center + Vector3.up * canStand_Collider.height * 0.3f;

        RaycastHit[] hit;                                                                    //decrease the radius a bit or else it will always hit

        
      //  Debug.DrawLine(p1, p2, Color.white);

        Collider[] boxHits;                                                 //reduce to 0.90 so that it works...
        boxHits = Physics.OverlapCapsule(p1, p2, canStand_Collider.radius * 0.90f, obstacleMask);
        
        if(boxHits.Length >= 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool CanUncrouchWhileInAir()
    {
        Vector3 p1 = transform.position + canUncrouchWhileInAir_Collider.transform.localPosition + Vector3.up * -canUncrouchWhileInAir_Collider.height * 0.3f;
        Vector3 p2 = transform.position + canUncrouchWhileInAir_Collider.transform.localPosition + Vector3.up * canUncrouchWhileInAir_Collider.height * 0.3f;

        RaycastHit[] hit;                                                                    //decrease the radius a bit or else it will always hit

        
        Debug.DrawLine(p1, p2, Color.white);

        Collider[] boxHits;                                                 //reduce to 0.90 so that it works...
        boxHits = Physics.OverlapCapsule(p1, p2, canUncrouchWhileInAir_Collider.radius * 0.90f, obstacleMask);

        canUncrouchColHitCount = boxHits.Length;
        if(boxHits.Length >= 1)
        {
            return false;
        }
        else
        {
            return true;
        }

    }


    //HEAD BOB AND FOOTSTEPSOUND MECHANIC
    public bool headBob_enabled = true;
    private float distanceTravelled_x = 0.0f;
    private float distanceTravelled_z = 0.0f;
    private float lastPosition_x;
    private float lastPosition_z;
    public float averageDistance_travelled;


    float headBob_yPosOffset = 0; //to add to the camera finaly y pos

    public float footStepResetDistance = 1f;
    public AnimationCurve headBob_yoffset_AnimationCurve;
    public AnimationCurve headBob_speedInfluence_AnimationCurve;
    
    public AudioSource footstep_audioSource;
    public AudioClip footstepSound;
    public AudioClip defautFootStepSound;


    //GOING FAST/FALLING EFFECTS
    public AudioSource fastWind_audioSource;
    public PostProcessVolume volume;
    ChromaticAberration chromaticAberration;

    public AnimationCurve fastSpeed_chromaticAbberation_AnimationCurve;
    public AnimationCurve fastSpeed_fastWind_AnimationCurve;
    public AnimationCurve fastSpeed_footStepSound_AnimationCurve;


    public AnimationCurve land_landPitch_AnimationCurve;
    public AnimationCurve land_landVolume_AnimationCurve;
    public AnimationCurve land_screenShake_AnimationCurve;
    public AudioSource fallLand_audioSource;
    public AudioClip fallLand_audioClip;




    //CLING MECHANIC

     public bool canTeleport = true;
    public AnimationCurve cling_MoveEase_AnimationCurve;



    public float cling_MaxDistance = 10f;
    public bool cling_blueBoxShapeCast_hitDetect;
    RaycastHit cling_shapeHit;

    Vector3 cling_rayDownStartPos;

    public BoxCollider boxTeleportCollider;
    public Vector3 boxTeleprtCast_ExtendsOffset; //for manually position the box cast size... any other way is unclear 
    public float boxTeleportCast_startOffset = 0.0f; //for making sure the box does not go trhough block up-close


    public bool canLedgeGrabOrTeleport = true; //this is used to prevent spamming.

    public float ledgeGrab_distance = 1.5f;

    public float TeleportDoMoveSpeed = 0.3f;
    public float LedgeGragDoMoveSpeed = 0.6f;

    public AudioSource teleport_Cling_AudioSource;
    public AudioSource teleport_Ticking_AudioSource;
    public GameObject teleport_Cling;

    public LineRenderer teleport_Line_Left;
    public LineRenderer teleport_Line_Right;

    public float ledgeGrapDepth = 0.0f;
    public float ledgeBoxPlacementDepth = 0.0f;
    public float ledgeBoxOffsetFromFloor = 0.015f; //to prevent invalid floor collision

    public AudioSource ledgeGrag_AudioSource;

    RaycastHit cling_hit_downPosition;
    public float cling_hit_down_StartHeight = 2f;
    bool cling_hit_downPositionFound = false;

    public bool cling_isThereABlockInTheWayOfLedgeClimbing;

    public float cling_hit_downDistance = 2f;

    Collider[] cling_boxHits;
    Vector3 cling_cubePos;

    public float minimumAngleWhereUserGrabsOntoFloor = 50.0f;

    public GameObject targetingMovement_Arrow;

  Tween cling_moveTween;



    //WALL JUMPING MECHANIC
    public bool canWallJump = false;
    public int wallJump_ComboCount = 0;

    public float wallJumpAmount = 1.0f;

    public AudioSource wallJump_audioSource;
    public AudioClip wallJump_audioClip;

    public GameObject blockAboutToBeUsed;
    private GameObject blockUsedToWallJump;



    //PREVENTING PLAYER FROM STICKING WHEN JUMPING WHILE RUNNING MECHANIC
    public void IsAllowedToBeGroundedAgain()
    {
        isAllowedToBeGroundedAgain_mod = true;
    }


    //CAN JUMP AGAIN MECHANIC
    public bool canJumpAgain = false; //used to let the player jump again in the air by setting this to true
    public float timeSincePressedToJump = 0.0f;
    public void ResetPlayerJump()
    {
        canJumpAgain = true;
    }



    //CRUSH MECHANIC  (uses a capsule to detect if an object entered)
    public PlayerMovementBasic_CrushDetectionCapsule crushDetectionCapsule;

public void DrawLedgeGrabCast()
  {

        //BLUE BOX (used to create a "realistic" teleportation, by using a box to simulate the size of the player)
        cling_blueBoxShapeCast_hitDetect = Physics.BoxCast(SimpleSmoothMouseLook.Instance.transform.position + SimpleSmoothMouseLook.Instance.transform.forward * boxTeleportCast_startOffset, boxTeleportCollider.size/2 + boxTeleprtCast_ExtendsOffset, SimpleSmoothMouseLook.Instance.transform.forward, out cling_shapeHit, transform.rotation, cling_MaxDistance, obstacleMask);


        //PROCEED TO DOING THE INWARDS -> UPWARDARS -> DOWNWARDS ray check

        // foreward from wall hit normal, and then upwards by 2 units
        cling_rayDownStartPos = (cling_shapeHit.point + -cling_shapeHit.normal * ledgeGrapDepth) + transform.up * cling_hit_down_StartHeight;

        //downward by "cling_hit_downDistance"
        cling_hit_downPositionFound = Physics.Raycast(cling_rayDownStartPos, -transform.up, out cling_hit_downPosition, cling_hit_downDistance, obstacleMask);


        //When a down position is found. proceed to checking via blue box "OverLap" get all objects shape cast
        if(cling_hit_downPositionFound)
       {
        
        //FOR CHECKING IF THE BLUE BOX HAS ANY BLOCKS IN IT


        //down ward hit point + normal offset + collider height offset + manually set offset from floor
       cling_cubePos = cling_hit_downPosition.point + (cling_shapeHit.normal * ledgeBoxPlacementDepth) + new Vector3(0,boxTeleportCollider.size.y/2,0) + new Vector3(0, ledgeBoxOffsetFromFloor,0);

       Vector3 p1 = cling_cubePos + canStand_Collider.center + Vector3.up * -canStand_Collider.height * 0.3f;
       Vector3 p2 = cling_cubePos + canStand_Collider.center + Vector3.up * canStand_Collider.height * 0.3f;
     
                                                                             //reduce to 0.90 so that it works...
        cling_boxHits = Physics.OverlapCapsule(p1, p2, canStand_Collider.radius * 0.90f, obstacleMask);
       //         boxHits = Physics.OverlapBox(cubePos , boxTeleportCollider.size/2, Quaternion.identity, ledgeGrabLayerMask);

       // Debug.Log("boxhits length:" + cling_boxHits.Length);


        //FOR CHECKING IF THERE IS ANYTHING BETWEEN THE RED AND BLUE BOX
             //check for any colliders from the center of the box cast shapes... to the center of the Overlap blue box shape
        Vector3 CenterOfBoxCast = SimpleSmoothMouseLook.Instance.transform.position + SimpleSmoothMouseLook.Instance.transform.forward * cling_shapeHit.distance + (SimpleSmoothMouseLook.Instance.transform.forward * boxTeleportCast_startOffset);
        RaycastHit cling_hit_isThereABlockInTheWay;
        cling_isThereABlockInTheWayOfLedgeClimbing = Physics.Raycast(CenterOfBoxCast, transform.up,  out cling_hit_isThereABlockInTheWay, 2.0f, obstacleMask);



        //If all of the requirements are met... 
        if(cling_hit_downPositionFound && cling_boxHits.Length <= 0 && maxSlopeAngleUntilPlayerSlides > minimumAngleWhereUserGrabsOntoFloor && cling_isThereABlockInTheWayOfLedgeClimbing == false)
        {       
            canTeleport = true;
            
          //DEBUGGING (Showing arrow)
          //targetingMovement_Arrow.SetActive(true);                
                                   //down hit point + normal offset + y offset (in order to position on an "edge")
          targetingMovement_Arrow.transform.position = cling_hit_downPosition.point + (cling_shapeHit.normal * 0.05f) + new Vector3(0,-0.9f,0);
          targetingMovement_Arrow.transform.rotation =Quaternion.LookRotation(cling_shapeHit.normal);
        }
        else
        {
          //DEBUGGING (Showing arrow)
          //targetingMovement_Arrow.SetActive(false);                

          //have to do it this way do to the UnHideTargeters() function
          targetingMovement_Arrow.transform.position = Vector3.zero;

        }

       }

  }



  //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
       if(Application.isPlaying)
       {
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (cling_blueBoxShapeCast_hitDetect)
        {

            //RED BOX THAT DETECTS ANYTHING IN THE WAY

            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(SimpleSmoothMouseLook.Instance.transform.position, SimpleSmoothMouseLook.Instance.transform.forward * cling_shapeHit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(SimpleSmoothMouseLook.Instance.transform.position + SimpleSmoothMouseLook.Instance.transform.forward * cling_shapeHit.distance + (SimpleSmoothMouseLook.Instance.transform.forward * boxTeleportCast_startOffset), boxTeleportCollider.size + boxTeleprtCast_ExtendsOffset);
      
      

            //BLUE CAPSULE THAT CHECKS IF THERE ARE OBJECTS IN THE DESIRED END POSITION
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(cling_rayDownStartPos, -transform.up * cling_hit_downDistance);
            Gizmos.DrawSphere(cling_hit_downPosition.point,0.1f);


            //Gizmos.DrawWireCube(cling_cubePos, boxTeleportCollider.size);

            Vector3 p1 = cling_cubePos + canStand_Collider.center + Vector3.up * -canStand_Collider.height * 0.3f;
            Vector3 p2 = cling_cubePos + canStand_Collider.center + Vector3.up * canStand_Collider.height * 0.3f;

                                        //reduce to 0.90 so that it works...
            Gizmos.DrawSphere(p1,canStand_Collider.radius * 0.90f);
            Gizmos.DrawSphere(p2,canStand_Collider.radius * 0.90f);


            //GREEN RAYCAST THAT CHECKS IF THERE IS ANYTHING IN THE WAY ON THE TOP
            Gizmos.color = Color.green;
            Vector3 CenterOfBoxCast = SimpleSmoothMouseLook.Instance.transform.position + SimpleSmoothMouseLook.Instance.transform.forward * cling_shapeHit.distance + (SimpleSmoothMouseLook.Instance.transform.forward * boxTeleportCast_startOffset);
            Gizmos.DrawRay(CenterOfBoxCast, transform.up * 2);



        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(SimpleSmoothMouseLook.Instance.transform.position, SimpleSmoothMouseLook.Instance.transform.forward * cling_MaxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(SimpleSmoothMouseLook.Instance.transform.position + SimpleSmoothMouseLook.Instance.transform.forward * cling_MaxDistance, boxTeleportCollider.size);
        }
       }
    }




    public void PositionPlayerOnCastEvent(Vector3 endPos)
    {
            rb.isKinematic = true;
            canTeleport = false;
            cling_moveTween.Kill();

            if(cling_boxHits.Length <= 0)
            {
            cling_moveTween = transform.DOMove(endPos,TeleportDoMoveSpeed).OnComplete(() => CornerGrabAnim()).SetEase(cling_MoveEase_AnimationCurve); 
            }
            else
            {
            cling_moveTween = transform.DOMove(endPos,TeleportDoMoveSpeed).OnComplete(() => OnEndDestReached()).SetEase(cling_MoveEase_AnimationCurve); 
            }
    }


    void HideTargeters()
    {
      targetingMovement_Arrow.SetActive(false);

    }

    void UnHideTargeters()
    {
      targetingMovement_Arrow.SetActive(true);
    }



    void CornerGrabAnim()
    {
        HideLineRenderersAndCling();

      ledgeGrag_AudioSource.Play();
      cling_moveTween = transform.DOMove(cling_hit_downPosition.point + (cling_shapeHit.normal * ledgeBoxPlacementDepth) + new Vector3(0,boxTeleportCollider.size.y/2,0), LedgeGragDoMoveSpeed).OnComplete(() => OnEndDestReached()); 
       }


    public void OnEndDestReached()
    {
                    rb.isKinematic = false;
                    crushDetectionCapsule.gameObject.SetActive(true);


        HideLineRenderersAndCling();

      canLedgeGrabOrTeleport = true;


    }
    
    public static Vector3 SphereOrCapsuleCastCenterOnCollision(Vector3 origin, Vector3 directionCast, float hitInfoDistance)
    {
        return origin + (directionCast.normalized * hitInfoDistance);
    }


    void HideLineRenderersAndCling()
    {
        teleport_Ticking_AudioSource.Stop();


        teleport_Cling.SetActive(false);
        teleport_Line_Left.transform.gameObject.SetActive(false);
        teleport_Line_Right.transform.gameObject.SetActive(false);

    }



    void UnHideLineRenderersAndCling()
    {
        teleport_Cling.SetActive(true);
        teleport_Line_Left.transform.gameObject.SetActive(true);
        teleport_Line_Right.transform.gameObject.SetActive(true);

        

                teleport_Cling_AudioSource.Play();
                teleport_Ticking_AudioSource.Play();

                teleport_Line_Left.SetPosition(0, teleport_Line_Left.transform.position);
                teleport_Line_Left.SetPosition(1, cling_shapeHit.point);

                teleport_Line_Right.SetPosition(0, teleport_Line_Right.transform.position);
                teleport_Line_Right.SetPosition(1, cling_shapeHit.point);

                teleport_Cling.transform.position = cling_shapeHit.point;

    }

    //CHECK FOR INCOMING SLOPES MECHANIC (for disable the fixed bhop box collider)
    public float checkForIncomingSlopesRay_positionOffset;
    public float checkForIncomingSlopesRay_length;




    // Update is called once per frame
    void FixedUpdate()
    {

        //GRINDING MECHANIC
            grinding_checkInterval_timer += Time.fixedDeltaTime;
        if (grinding_checkInterval_timer >= grinding_checkInterval)
            {
                // Your code here
                grinding_checkInterval_timer = 0;
                CheckForSharpTurn();
            }
                






                //do this in fixed update or else there WILL be inconistencies...
                //do this early in fixed update or else for some reason... hopping on top of ramp tips/sharp points wont work...    
            
                //AUTO BHOP MECHANIC
                if(isGrounded == true && holdingJump && jumpInputPressThreshold >= 1)
                {   
                    if(isCrouching == true && CanStandUp() == false)
                    {
                     //   Debug.Log("11111");
                    }
                    else
                    {
                     //   Debug.Log("22222");
                    //play jump sound
                    EvaluateFootStepSound();
                    footstep_audioSource.pitch = 1.2f;
                    footstep_audioSource.PlayOneShot(footstepSound, 1.0f);


                    RemovePlayerFromPlatform();



                    isGrounded = false;
                    transform.position += Vector3.up * 0.1f;

                    //used to prevent player from sticking to ground while running
                    isAllowedToBeGroundedAgain_mod = false;
                    Invoke("IsAllowedToBeGroundedAgain", 1f);

                    //if the player is going down... make the y velocity 0... so that they feel a jump
                    if(rb.velocity.y < 0)
                    {
                    rb.velocity = new Vector3(rb.velocity.x,0,rb.velocity.z);
                    }


                    canWallJump = false;

                   // rb.velocity = new Vector3(rb.velocity.x,(transform.up * jumpForce).y ,rb.velocity.z);


                    //bhop add velocity
                    Vector3 forwardForce = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized; //make sure to normalize or else the player will go slower up/down hill
                    Vector3 rightForce = Vector3.ProjectOnPlane(transform.right, hit.normal).normalized;

                    desiredVelocity = (movement.z * forwardForce + movement.x * rightForce) * 7; //multiply by magnitude to send the player in a direction with momentum
                    rb.velocity = new Vector3(rb.velocity.x, (transform.up * jumpForce).y ,rb.velocity.z);


                    }
                }












        //CROUCHING MECHANIC



        if(Input.GetKey(KeyCode.LeftControl))
        {
            if(isCrouching == false)    //crouch...
            {
                        isCrouching = true;


                        //air crouching (moves player up)
                        if(isGrounded == false) //to prevent the player from spamming crouch in the air
                        {
                                                            crouch_yPosOffset = crouch_yPosOffsetCamera;

                            transform.position += new Vector3(0, Mathf.Abs(crouch_yPosOffsetCamera),0);
                        }

                        //resize player capsule
                        GetComponent<CapsuleCollider>().height = 0.8f;
                        GetComponent<CapsuleCollider>().center = new Vector3(0,-0.4f,0);


                        //resize crush detection capsule too
                        crushDetectionCapsule.capsuleCollider.height =  0.8f * crushDetectionCapsule.shrinkMultiplier;
                        crushDetectionCapsule.capsuleCollider.center =  new Vector3(0,-0.4f,0);
            }

        }
        else
        {
            if(isCrouching == true)
            {
                    //moves the player back down if they can uncrouch in the air. (similar to source game flickering that happens when spamming crouch in air)
                   if(CanUncrouchWhileInAir())
                {
                    isCrouching = false;

                        if(isGrounded == false) //to prevent the player from spamming crouch in the air
                        {
                                    crouch_yPosOffset = 0;
                            transform.position -= new Vector3(0, Mathf.Abs(crouch_yPosOffsetCamera),0);
                        }


                    //resize player capsule
                    GetComponent<CapsuleCollider>().height = 1.6f;
                    GetComponent<CapsuleCollider>().center = new Vector3(0,0,0);      

                    //resize crush detection capsule too
                    crushDetectionCapsule.capsuleCollider.height = 1.6f * crushDetectionCapsule.shrinkMultiplier;
                    crushDetectionCapsule.capsuleCollider.center = new Vector3(0,0,0);
                }
                if(isGrounded == true && CanStandUp())
                {
                        isCrouching = false;

                        if(isGrounded == false) //to prevent the player from spamming crouch in the air
                        {
                            transform.position -= new Vector3(0, 0.5f,0);
                        }


                    //resize player capsule
                    GetComponent<CapsuleCollider>().height = 1.6f;
                    GetComponent<CapsuleCollider>().center = new Vector3(0,0,0);      

                    //resize crush detection capsule too
                    crushDetectionCapsule.capsuleCollider.height = 1.6f * crushDetectionCapsule.shrinkMultiplier;
                    crushDetectionCapsule.capsuleCollider.center = new Vector3(0,0,0);
                }
            }
        }

        //crouch modifiers
                    /* //scrapped crounching slide mechanic

        if(isCrouching)
        {
            maxSlopeAngleUntilPlayerSlides = maxSlopeAngleUntilPlayerSlides_Crouched;


            //to prevent the crouching gravity from being applied in the air (causing the player to completely sink)
            if(isGrounded)
            {
            Physics.gravity = gravity_Crouched;
            }
            else
            {
            Physics.gravity = gravity_Standing;
            }

            GetComponent<CapsuleCollider>().material = physicsMaterial_Crouched;
            maxSpeed = maxSpeed_Crouching;
        }
        else
        {
            maxSlopeAngleUntilPlayerSlides = maxSlopeAngleUntilPlayerSlides_Standing;
            Physics.gravity = gravity_Standing;
            GetComponent<CapsuleCollider>().material = physicsMaterial_Standing;
            maxSpeed = maxSpeed_Standing;
        }
        */

        //animate the crouch camera thing
        if(isCrouching == true )
        {
            crouch_yPosOffset = Mathf.MoveTowards(crouch_yPosOffset, crouch_yPosOffsetCamera, Time.deltaTime * 5.0f);
        }
        else
        {
            crouch_yPosOffset = Mathf.MoveTowards(crouch_yPosOffset, 0, Time.deltaTime * 5.0f);

        }

        //HEAD BOB AND FOOTSTEPSOUND MECHANIC

        //averate distance travelled on x and z axis


        //when a player is on a platform... make sure to use the player's local position! or else while moving on a platform... the player will "walk" !!!
        if(platformMechanic_currentPlatform == null)
        {
            distanceTravelled_x += Mathf.Abs(transform.position.x - lastPosition_x);
            distanceTravelled_z += Mathf.Abs(transform.position.z - lastPosition_z);

            lastPosition_x = transform.position.x;
            lastPosition_z = transform.position.z;
        }
        else    
        {
            distanceTravelled_x += Mathf.Abs(transform.localPosition.x - lastPosition_x);
            distanceTravelled_z += Mathf.Abs(transform.localPosition.z - lastPosition_z);


            lastPosition_x = transform.localPosition.x;
            lastPosition_z = transform.localPosition.z;
        }

        averageDistance_travelled = (distanceTravelled_x + distanceTravelled_z)/2;
      

        //make a sound every certain distance covered
        if(averageDistance_travelled > footStepResetDistance && isGrounded)
        {
            distanceTravelled_x = 0.0f;
            distanceTravelled_z = 0.0f;

            EvaluateFootStepSound();
            footstep_audioSource.pitch = Random.Range(0.9f, 1.1f);
            footstep_audioSource.PlayOneShot(footstepSound, 1.0f);
        }

        //apply head bob depending on the footstep distance normal

        if(headBob_enabled)
        {
        float influenceToApplyDependingOnSpeed = headBob_speedInfluence_AnimationCurve.Evaluate(speed);
        headBob_yPosOffset = headBob_yoffset_AnimationCurve.Evaluate(averageDistance_travelled/footStepResetDistance) * influenceToApplyDependingOnSpeed;
        }
        else
        {
        headBob_yPosOffset = 0;
        }




        //APPLYING THE Y OFFSET EFFECTS TO CAMERA (HEADBOB AND CROUCHING)
        float yPosOfAllEffects = cameraStartLocalYPos + headBob_yPosOffset + crouch_yPosOffset;
        playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, yPosOfAllEffects ,playerCamera.transform.localPosition.z);





        //GOING FAST/FALLING EFFECTS (POST PROCESSING AND SOUNDS)
        chromaticAberration.intensity.value = fastSpeed_chromaticAbberation_AnimationCurve.Evaluate(speed);
        fastWind_audioSource.volume = fastSpeed_fastWind_AnimationCurve.Evaluate(speed);
        footstep_audioSource.volume = fastSpeed_footStepSound_AnimationCurve.Evaluate(speed);


        


        //MOVEMENT LOGIC (RAY CAST TO CHECK FLOOR SLOPE, RUNNING, WALKING, ETC.)

        //if running... make it easier for the player to be grounded
        //this is mostly used for outward loops
         if(Physics.Raycast(transform.position, -transform.up,  out hit, raycastGroundCheckDistance, obstacleMask))
                {
                    currentHittingRaySlopeAngle = Vector3.Angle(Vector3.up, hit.normal);

                    //IMPORTANT! This checks if the current slope the ray is hitting isnt a floor... if it has any amount of tilt... then assume the player will want to allign with it when running
                    if(currentHittingRaySlopeAngle > 0)
                    {
                        if(isRunning && isAllowedToBeGroundedAgain_mod)
                        {
                            isGrounded = true;
                        }
                    }

                        //used to prevent the player from sticking onto steep angles. (like when running off of a ramp)
                        //the problem was: related to how the player downward ray would always use the angle of the face that was downwards. sometimes it would be angled too differently than the current standing face and cause the player to flicker in place. 
                        
                        //Note: if you ever have problems with ramps... check this area

                        if(Mathf.Abs(Mathf.Abs(currentHittingRaySlopeAngle) - Mathf.Abs(lastHittingSlopeAngle)) > maxSlopeAngleUntilPlayerSlides)
                        {
                            isGrounded = false; 
                        }
                        else
                        {
                        lastHittingSlopeAngle = currentHittingRaySlopeAngle;
                        }
                

        }

        //do this here or else... when the player falls off a slope at a steep angle and they get back on flat ground(0 slope) they will have incorrect behavior because of "lastHittingSlopeAngle"
     
        //Note: if you ever have problems with ramps... check this area
        if(isGrounded == false)
        {
            lastHittingSlopeAngle = 0;
        }




        //Grounded = Walk Or Running
        //Ungrounded = InAir, Jumping, Or Sliding
        if(isGrounded)
        {
            if(landEventSetToTrigger == true)
            {   
                Debug.Log("LAND EVENT  HAPPENED");

                    //use velocity direction normall AND... floor normal? 
           
                    
                    accumilatedSpeed = rb.velocity.magnitude;
                   
                    //To prevent the player from speeding up by jumping and landing
                    if(new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude <= maxSpeed + 2)
                    {
                        accumilatedSpeed = 0;
                    }

                    //play land sound
                    EvaluateFootStepSound();
                    footstep_audioSource.pitch = 0.8f;
                    footstep_audioSource.PlayOneShot(footstepSound, 1.0f);


                    //fall effects
                    fallLand_audioSource.pitch = land_landPitch_AnimationCurve.Evaluate(Mathf.Abs(previousVelocity.y));
                    fallLand_audioSource.volume = land_landVolume_AnimationCurve.Evaluate(Mathf.Abs(previousVelocity.y));
                    fallLand_audioSource.PlayOneShot(fallLand_audioClip, 1.0f);

                    ScreenShake2.Instance.Shake(land_screenShake_AnimationCurve.Evaluate(Mathf.Abs(previousVelocity.y)));


                   
                landEventSetToTrigger = false;
            }
            //when on ground... add/remove velocity depending on slope
            SpeedAccumillationLogic();

            
            if(speed > speedToWallRun)
            {

                //this check is added so that when the player lands on a block while bhopping... they are able to still bhop without getting interrupted
                if(isStrafing == false)
                {
                isRunning = true;
                WallRunningControlls();
                }
                
            }
            else
            {
                isRunning = false;
                NormalWalkControlls();
            }

            
        }
        else
        {
            RemovePlayerFromPlatform();



            landEventSetToTrigger = true;
            AirControlls();
   
            //when in air... convery velicity to accumiliatedSpeed
            //NEEDS WORK!!!!
            accumilatedSpeed = velocity.magnitude;
            
        }     

      //  Debug.Log("Rigidbody player vel: " + rb.velocity);



        //this is a hardcoded platforming thing... its done so that when the player hits a wall(low velocity) the accumilated speed also "realistically" gets set to 0
        //NEEDS IMPROVEMENT! too easy to maintain speed still!!!
        if(velocity.magnitude <= 0)
        {
            accumilatedSpeed = 0f;
        }


               
        ClearState ();
        prevNormal = hit.normal;
        
        //should this be located at the beggining or ending or fixedupdate()?
        speed = rb.velocity.magnitude;
        previousVelocity = rb.velocity;
    }


    public bool landEventSetToTrigger = false;

    void SpeedAccumillationLogic()
    {
        //flat ground
        if(slopeSteepness == 90) // completely stopped
        {
         //had to comment this line out because when used with events to position the player running on a wall sideways... it would set the accumilatedSpeed to zero...
         //  accumilatedSpeed -= slopeSteepness * Time.deltaTime * accumilationSpeed * 100f; 
        }
        else if(slopeSteepness == 0) //flat ground
        {
            accumilatedSpeed -=  Time.deltaTime * accumilationSpeed * loseSpeed_flatGround;
        }
        else //slopes
        {
            if(movingUphill)
            {
                accumilatedSpeed -= slopeSteepness * Time.deltaTime * loseSpeed;
            }
            else
            {
                accumilatedSpeed += slopeSteepness * Time.deltaTime * accumilationSpeed;
            }
        }

            accumilatedSpeed = Mathf.Clamp(accumilatedSpeed, 0, accumilatedSpeed_max);

    }


    public Vector3 desiredVelocity;
    public float maxSpeed;
    public float maxSpeed_Standing;
    public float maxSpeed_Crouching;
    private void NormalWalkControlls()
    {
            rb.useGravity = false;

            control = speedControl.Evaluate(speed);
            
            //note: this is what makes the player quickly snap into movement on input... and "lerp" out when not pressed... (because less control)
            control += speedControl_playerInput.Evaluate(movement.magnitude);
            control = control / 2; //for getting the avaerage


            /* //scrapped crounching slide mechanic
            if(isCrouching)
            {
                rb.useGravity = true;
                if(movement.magnitude > 0.1)
                {

                }
                else
                {
                control *= 0.15f;
                }
            }
        */

            maxSpeedChange = control * Time.deltaTime * 100;




            //set x and z rotation back to 0
            float currentYRotation = transform.rotation.eulerAngles.y;
            Vector3 newEulerAngles = new Vector3(0,0,0);
            newEulerAngles.y = currentYRotation; // Set the Y rotation back to the current Y rotation
            Quaternion targetRotation = Quaternion.Euler(newEulerAngles);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);

            //move player depending on surface normal (and player foreward)
            Vector3 forwardForce = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized; //make sure to normalize or else the player will go slower up/down hill
            Vector3 rightForce = Vector3.ProjectOnPlane(transform.right, hit.normal).normalized;

            desiredVelocity = (((movement.z * forwardForce + movement.x * rightForce) * maxSpeed)  ) * (1 + accumilatedSpeed * accumilatedSpeed_influence) ;

 


/*
           //Slowly pulls player toward the floor...
            //the longer the distance from the floor. the faster the downward added velocity
            float distanceFromBottomOfCapToRayHit = Vector3.Distance(hit.point, transform.position + -transform.up * 0.8f);
            desiredVelocity += -transform.up * distanceFromBottomOfCapToRayHit * 1000;
            Debug.Log("distanceFromBottomOfCapToRayHit: " + distanceFromBottomOfCapToRayHit);
  */          


            Debug.DrawRay(transform.position, desiredVelocity * 2, Color.green);

            velocity = rb.velocity;

            //the higher the "maxSpeedChange"... the more quickly the player's movement input effects the whole velocity
            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
            velocity.y = Mathf.MoveTowards(velocity.y, desiredVelocity.y, maxSpeedChange);

            ConvertVelocityRelativeToNormal();

            rb.velocity =  velocity;

            CalculateIfPlayerGoingUpHill();

    }    

    

    private void WallRunningControlls()
    {
            rb.useGravity = false;

            control = speedControl.Evaluate(speed);

                                                // the "&& verticalInput >= 0.1" is used to prevent that "bug/mechanic" where the player does not lose speed when they let go of foreward...
            if(enableLessFrictionOnNoInput && verticalInput >= 0.1)
            {
                control += speedControl_playerInput.Evaluate(movement.magnitude);

                control = control / 2; //for getting the avaerage
            }
            

            maxSpeedChange = control * Time.deltaTime * 100;



            //rotate player depending on surface normal
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            //transform.rotation = targetRotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationLerpSpeed);



            //move player depending on surface normal (and player foreward)
            Vector3 forwardForce = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized;
            Vector3 rightForce = Vector3.ProjectOnPlane(transform.right, hit.normal).normalized;

            desiredVelocity = (((movement.z * forwardForce + movement.x * rightForce) * maxSpeed)  ) * (1 + accumilatedSpeed * accumilatedSpeed_influence);
 
            //Slowly pulls player toward the floor...
            //the longer the distance from the floor. the faster the downward added velocity
            float distanceFromBottomOfCapToRayHit = Vector3.Distance(hit.point, transform.position + -transform.up * 1f);
            desiredVelocity += -transform.up * distanceFromBottomOfCapToRayHit;
            
            
            velocity = rb.velocity;

            
            ConvertVelocityRelativeToNormal();

            //the higher the "maxSpeedChange"... the more quickly the player's movement input effects the whole velocity
            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
            velocity.y = Mathf.MoveTowards(velocity.y, desiredVelocity.y, maxSpeedChange);
       //     velocity.y = Mathf.Lerp(velocity.y, desiredVelocity.y * 10, 1);

            rb.velocity =  velocity;

            CalculateIfPlayerGoingUpHill();
    }



    //gap variables
    //These are used to prevent a velocity change from happening when the normal hit is too different from the previous normal. 
    //(very useful for preventing the player from stopping just because the raycast used a minor gap's normal)

    //Example: If the new normal is only 40.0f degree difference... then convert the velocity... otherwise... continue with current velocity
    public float maxDifferenceBetweenAngles = 40.0f;
    public Vector3 prevNormal;
    public float angleDiff_Debug;

    private void ConvertVelocityRelativeToNormal()
    {
        angleDiff_Debug = Vector3.Angle(hit.normal, prevNormal);
      //  Debug.Log("ANGLE FIFF: " + angleDiff_Debug);   
        //only convert the velocity if its under a certain angle... [See gap variables]
        if(Vector3.Angle(hit.normal, prevNormal) < maxDifferenceBetweenAngles)
        {

        float parallelComponent = Vector3.Dot(velocity, hit.normal);

        Vector3 perpendicularComponent = velocity - (parallelComponent * hit.normal);

        //old way 
        //Vector3 forwardVelocityForce = Vector3.ProjectOnPlane(velocity, hit.normal);
        velocity = perpendicularComponent;
        }
    }
    private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float max_velocity)
    {


    float projVel = Vector3.Dot(prevVelocity, accelDir); // Vector projection of Current velocity onto accelDir.
    float accelVel = accelerate * Time.fixedDeltaTime; // Accelerated velocity in direction of movment

    // If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
    if(projVel + accelVel > max_velocity)
        accelVel = max_velocity - projVel;

    Vector3 flatPlaneVel = new Vector3(prevVelocity.x, 0, prevVelocity.z);

    Vector3 directionBetween = Vector3.Lerp(accelDir.normalized, flatPlaneVel.normalized, 0.5f);

    Vector3 directionBetween_foreward_and_inputDir = Vector3.Lerp(transform.forward, accelDir, 0.5f);


            Debug.DrawRay(transform.position, accelDir * 6, Color.yellow);
            Debug.DrawRay(transform.position, flatPlaneVel * 6, Color.red);
 //           Debug.DrawRay(transform.position, directionBetween * 6, Color.yellow);



    return prevVelocity + accelDir * accelVel;



    }
    public bool isStrafing = false;
    public float quakStrafe_accelerate = 32f;
    public float max_velocity = 3f;
    public float sideStrafeAcceleration = 100;
    private void AirControlls()
    {
    rb.useGravity = true;
            maxSpeedChange = speedControl_air.Evaluate(speed);

            //set x and z rotation back to 0
            float currentYRotation = transform.rotation.eulerAngles.y;
            Vector3 newEulerAngles = new Vector3(0,0,0);
            newEulerAngles.y = currentYRotation; // Set the Y rotation back to the current Y rotation
            Quaternion targetRotation = Quaternion.Euler(newEulerAngles);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);




            //make sure to NOT use the usual "forward/right" forces(that use hit.normal). because if you do... you will have problems leaving a ramp while surfing... AND have problems with bhop around ramps
            Vector3 forwardForce = transform.forward;
            Vector3 rightForce = transform.right;

            desiredVelocity = (movement.z * forwardForce + movement.x * rightForce) * maxSpeed * (1 + accumilatedSpeed * accumilatedSpeed_influence_air);

            velocity = rb.velocity;

            //the higher the "maxSpeedChange"... the more quickly the player's movement input effects the whole velocity
            //only change the x and z when falling
            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);






        float accel = 1;



                // If the player is ONLY strafing left or right    //do NOT use "isStrafing"... causes shitty wall jumping for some reason
                if(verticalInput == 0 && horizontalInput != 0)
                {
                    accel = sideStrafeAcceleration;
                                rb.velocity = Accelerate(desiredVelocity.normalized, rb.velocity, sideStrafeAcceleration, max_velocity);

                }
                else
                {
                                 rb.velocity =  velocity;
                }






    }

        //This is done by getting the angle the force in going in and using hit.normal
        private void CalculateSlopeSteepness()
        {
                Vector3 forwardVelocityForce = Vector3.ProjectOnPlane(rb.velocity.normalized, hit.normal);
                slopeSteepness = Vector3.Angle(Vector3.up, forwardVelocityForce);
                // If the angle is greater than 90 degrees, subtract it from 180 to get the equivalent angle in the range of 0 to 90 degrees
                if (slopeSteepness > 90f)
                {
                    slopeSteepness = 180f - slopeSteepness;
                }
                    // 0 = not steep, 90 = vertical
                    slopeSteepness = 90f - slopeSteepness;
        }


        private void CalculateIfPlayerGoingUpHill()
        {
            //Calculate if player is going up or down hill
            float deltaElevation = transform.position.y - previousPosition.y;
            movingUphill = deltaElevation > 0.0f;
  
           // Update the previous position for the next frame
            previousPosition = transform.position;
        }


        public float horizontalInput;
        public float verticalInput;
        public bool holdingJump = false;
        public Vector3 movement;
        public int jumpInputPressThreshold = 0; //used to make sure the player does not jump when they press space to confirm dialogue choice


        private void GetInputValues()
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            movement = new Vector3(horizontalInput, 0f, verticalInput);
            movement.Normalize(); //for making sure player cannot go faster holding horizontal and vertical keys

            holdingJump = Input.GetKey(KeyCode.Space);

            if(Input.GetKeyDown(KeyCode.Space))
            {
                jumpInputPressThreshold++;
            }
            
        }

    public GameObject boxColliderThatStaysInPlaceForBhopping;


        void OnCollisionEnter (Collision collision) 
    {
		EvaluateCollision(collision);

     
	}

	void OnCollisionStay (Collision collision) 
    {
		EvaluateCollision(collision);
	}

    //used to determine if the player is colliding with any "floor" layers.
    //"floor layers" = contacts that are under "maxSlopeAngleUntilPlayerSlides"
    //NOTE: the count gets reset in the ClearState() function. Its important to do that there because "OnCollisionStay" is called for EVERY collider... so setting the floorCollisionCount to 0 in "EvaluateCollision" will yield problems!!!
	int floorCollisionCount = 0;

    public float minGroundDotProduct;
	void EvaluateCollision(Collision collision)
    {
            //used to get the average vector normal of all contacts
		    minGroundDotProduct = Mathf.Cos(maxSlopeAngleUntilPlayerSlides * Mathf.Deg2Rad);
        		for (int i = 0; i < collision.contactCount; i++) 
                {
                    contactPoint = collision.GetContact(i).point;

			Vector3 normal = collision.GetContact(i).normal;

            //makes sure to only detect contact the player can actualy stand on (under the "maxSlopeAngleUntilPlayerSlides" value)
			if (normal.y >= minGroundDotProduct) 
            {
				contactNormal += normal;
                floorCollisionCount++;
			}
                }
            contactNormal.Normalize();
   
            Debug.DrawRay(transform.position, contactNormal * 2, Color.blue);

            //very similar to "slope angle", except it only uses valid normals under the "maxSlopeAngleUntilPlayerSlides" value
            float normalAngle = Vector3.Angle(Vector3.up, contactNormal);
        //    Debug.Log("normalAngle" + normalAngle);
            //if there are floor collision contacts... then player is not grounded
            if(floorCollisionCount >= 1)
            {
                if(normalAngle > maxSlopeAngleUntilPlayerSlides)
                {
                    isGrounded = false;
                }
                else
                {
                    isGrounded = true;

                        if(collision.gameObject.GetComponent<GeneralObjectInfo>().CheckIfObjectIsBeingPlayedOnPath())
                        {   
                            //this line is to make sure player only gets parented when the thing they touched is beneath them... the "- FLOAT" is just an appromimate. its the exact point where the player capsule would fall off if standing too near the edge...
                            if(collision.GetContact(0).point.y < transform.position.y - 0.68f)
                            {
                                if(platformMechanic_currentPlatform != collision.gameObject)
                                {
                                    AttachPlayerToPlatform(collision.gameObject);
                                }
                            }
                        }
                        if(platformMechanic_currentPlatform != collision.gameObject)
                        {
                            //this line is to make sure player only gets parented when the thing they touched is beneath them... the "- FLOAT" is just an appromimate.  its the exact point where the player capsule would fall off if standing too near the edge...
                            if(collision.GetContact(0).point.y < transform.position.y - 0.68f)
                            RemovePlayerFromPlatform();
                        }
                }
            }
            else
            {
                isGrounded = false;
            }


    }

    

    public Vector3 contactNormal;
    public Vector3 contactPoint; //mostly used just for footstep face detection

    //SPEED ADDING EVENTS
    public void AddVelocity(Vector3 velocity)
    {
        rb.velocity += velocity;
        accumilatedSpeed = rb.velocity.magnitude;
    }

    public void SetVelocity(Vector3 velocity)
    {
        rb.velocity = velocity;
        //rb.AddForce(velocity, ForceMode.VelocityChange); //the old way... couldnt be used because didnt work well with events... (player would fall off on wall run)
       
        accumilatedSpeed = velocity.magnitude;
        speed = velocity.magnitude;
        isRunning = true;
        isAllowedToBeGroundedAgain_mod = true;
        isGrounded = true;

        landEventSetToTrigger = false; // or else for some reason... it will fuck up the accumilatedspeed
    }

    public void SetVelocity_Launch(Vector3 velocity)
    {
        transform.position += Vector3.up * 0.1f;
        rb.velocity = new Vector3(0,0,0);
        rb.AddForce(velocity, ForceMode.VelocityChange);
        accumilatedSpeed = velocity.magnitude;

    }




    //FOOTSTEP
    public void EvaluateFootStepSound()
    {
        //to determine which sound to play... detect the contact point of collision where the capsule is touching
        //shoot a raycast there to get the triangle index


            RaycastHit footStepRayCastHit;

            Vector3 direction = contactPoint - transform.position;
            direction.Normalize();

            //not a very accurate raycast... but it works
            if(Physics.Raycast(transform.position, direction,  out footStepRayCastHit, raycastGroundCheckDistance, obstacleMask))
            {
                if(footStepRayCastHit.transform.GetComponent<BlockFaceTextureUVProperties>())
                footstepSound = footStepRayCastHit.transform.GetComponent<BlockFaceTextureUVProperties>().GetAudioClipFromFaceBasedOnTriangleIndex(footStepRayCastHit.triangleIndex);
           
                if(footStepRayCastHit.transform.GetComponent<PosterMeshCreator>())
                footstepSound = footStepRayCastHit.transform.GetComponent<PosterFootstepSound>().footStepAudioClip;

            }

            if(footstepSound == null)
            {
                footstepSound = defautFootStepSound;
            }

    }




}
