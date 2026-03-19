using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using SFB;
using DG.Tweening;
using TMPro;

public class PlayerObjectInteractionState_Equipped_Scanner : IPlayerObjectInteractionState 
{
        bool entered = false;
        bool clickedToExit = false;
        string filePathToSaveImages;

        float scannedAmount = 0.0f;
        bool scannedObjectReseter = false; //used to make the player press down again to start scanning another thing

        float distanceBetweenPlayerAndPoster = 0.0f;

    public IPlayerObjectInteractionState DoState(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(entered == false)
        {
            OnEnter(playerObjectInteraction);
            entered = true;
        }

        if(clickedToExit == true)
        {
            OnExit(playerObjectInteraction);
            return playerObjectInteraction.PlayerObjectInteractionStateIdle;
        }


        OnStay(playerObjectInteraction);

        return playerObjectInteraction.PlayerObjectInteractionStateEquippedScanner;
    }


    
        void OnEnter(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        playerObjectInteraction.scanner_model.SetActive(true);
        playerObjectInteraction.toolTipText.text = "Left Mouse: <color=green>Scan <color=white>| RIght Mouse: <color=red>Cancel <color=white> | Shift: <color=yellow> open saved media folder";
  
        //enable xray
        XrayRaycast.Instance.enabled = true;
    }



        RaycastHit[] allHits;

        GameObject closestGameObjectThatIsScannable;



        //this makes sure to ignore materials that have xray properties (by checking if _size is greather than 0)
        GameObject GetClosestScannableGameObject(PlayerObjectInteractionStateMachine playerObjectInteration)
        {
            for (int i = 0; i < allHits.Length; i++)
            {
                if(allHits[i].transform.GetComponent<IsScannable>() && allHits[i].transform.GetComponent<IsScannable>().isScannable == true)
                {
                return allHits[i].transform.gameObject;
                }

                if(allHits[i].transform.GetComponent<Block>())
                {
                    if(allHits[i].transform.GetComponent<BlockFaceTextureUVProperties>().CheckIfFaceHasXrayPropertiesBasedOnIndex(allHits[i].triangleIndex) == false)
                    return null;
                }

                if(allHits[i].transform.GetComponent<PosterMeshCreator>())
                {
                    if(allHits[i].transform.GetComponent<PosterMeshCreator>().CheckIfPosterMaterialHasXrayProperties() == false)
                    return null;
                }

            }
            return null;
        }


        void OnStay(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
                 allHits = Physics.RaycastAll(playerObjectInteraction.gameObject.transform.position, playerObjectInteraction.gameObject.transform.forward, XrayRaycast.Instance.maxDistance, playerObjectInteraction.collidableLayers_layerMask);
                  
                    System.Array.Sort(allHits, (x,y) => x.distance.CompareTo(y.distance));

                    //only allow scanning if an object that is scannable and not obstructed was found.          AND that mouse down reseter thing...
                    if(GetClosestScannableGameObject(playerObjectInteraction) && scannedObjectReseter == false)
                    {

                                //on gameobject null or different gameobject... reset scanner amount and assign new object
                                if(playerObjectInteraction.objectCurrentlyScanning == null || playerObjectInteraction.objectCurrentlyScanning != GetClosestScannableGameObject(playerObjectInteraction))
                                {
                                    playerObjectInteraction.objectCurrentlyScanning = GetClosestScannableGameObject(playerObjectInteraction);
                                    scannedAmount = 0.0f;
                                }

                                PositionTickers(playerObjectInteraction);                             

                                         if(Input.GetMouseButton(0) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
                                        {
                                             if(playerObjectInteraction.scannerScan_AudioSource.isPlaying == false)
                                             {
                                                playerObjectInteraction.scanner_spinner.transform.localRotation = Quaternion.Euler(0,-13.862f, 0);
                                                DOTween.Kill(playerObjectInteraction.scanner_spinner.transform);
                                                playerObjectInteraction.scanner_spinner.transform.DOShakeRotation(2.5f, new Vector3(0,12,0), 22, 90,false);
                                                playerObjectInteraction.scannerScan_AudioSource.Play();
                                             }
                                              distanceBetweenPlayerAndPoster = Vector3.Distance(playerObjectInteraction.playerCamera.transform.position, playerObjectInteraction.objectCurrentlyScanning.transform.position);
                                              scannedAmount += Time.deltaTime;
                                              playerObjectInteraction.ScanAmount_Text.text = "Scanning...";
                                        }
                                        else
                                        {
                                             if(playerObjectInteraction.scannerScan_AudioSource.isPlaying == true)
                                             {
                                                DOTween.Kill(playerObjectInteraction.scanner_spinner.transform);
                                                playerObjectInteraction.scannerScan_AudioSource.Stop();
                                             }
                                            playerObjectInteraction.ScanAmount_Text.text = "";
                                            playerObjectInteraction.scannerScan_AudioSource.Stop();
                                            scannedAmount = 0.0f;
                                        }

                     
                            
                    }
                    else 
                    {
                                if(playerObjectInteraction.scannerScan_AudioSource.isPlaying == true)
                                {
                                DOTween.Kill(playerObjectInteraction.scanner_spinner.transform);
                                playerObjectInteraction.scannerScan_AudioSource.Stop();
                                }
                                scannedAmount = 0.0f;
                                playerObjectInteraction.scannerScan_AudioSource.Stop();
                                HideTickers(playerObjectInteraction);
                    }


                //fully scanned event
                if(scannedAmount >= 2.5f && scannedObjectReseter == false)
                {
                    if(playerObjectInteraction.scannerScan_AudioSource.isPlaying == true)
                    {
                    DOTween.Kill(playerObjectInteraction.scanner_spinner.transform);
                    playerObjectInteraction.scannerScan_AudioSource.Stop();
                    }
                    scannedAmount = 0.0f;
                    scannedObjectReseter = true;

                    playerObjectInteraction.objectCurrentlyScanning.GetComponent<IsScannable>().OnScanned();
                }
   

            if(Input.GetMouseButtonDown(0) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
           {
             scannedObjectReseter = false;
           }


            if(Input.GetMouseButtonDown(1) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
           {
               clickedToExit = true;  
           }
        

            if(Input.GetKeyDown(KeyCode.LeftShift) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
            {
                if(!Directory.Exists(Application.persistentDataPath + "/SavedMedia/"))
                {    
                    Directory.CreateDirectory(Application.persistentDataPath + "/SavedMedia/");
                }
                string itemPath = Application.persistentDataPath + "/SavedMedia/";
                Application.OpenURL(itemPath);          
            }


        }

        void OnExit(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
                HideTickers(playerObjectInteraction);

                entered = false; // reset
                playerObjectInteraction.pickedUpObject = false;
                clickedToExit = false;
                playerObjectInteraction.scanner_model.SetActive(false);

                //enable xray
                XrayRaycast.Instance.enabled = false;
        }


    void PositionTickers(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
    //    playerObjectInteraction.UpperLeft_CornerHUD.SetActive(true);
    //    playerObjectInteraction.UpperRight_CornerHUD.SetActive(true);
    //    playerObjectInteraction.LowerLeft_CornerHUD.SetActive(true);
    //    playerObjectInteraction.LowerRight_CornerHUD.SetActive(true);
        playerObjectInteraction.ScanInfo_HUD.SetActive(true);

        Bounds bounds = playerObjectInteraction.objectCurrentlyScanning.GetComponent<Renderer>().bounds; // Get the bounds of the object's renderer

        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents * 1.2f;


        // Calculate positions for the selection boxes
       // Vector3 topLeft = new Vector3(center.x - extents.x, center.y + extents.y, center.z - extents.z);
       // Vector3 topRight = new Vector3(center.x + extents.x, center.y + extents.y, center.z - extents.z);
       // Vector3 bottomLeft = new Vector3(center.x - extents.x, center.y - extents.y, center.z - extents.z);
       // Vector3 bottomRight = new Vector3(center.x + extents.x, center.y - extents.y, center.z - extents.z);        


    //    playerObjectInteraction.UpperLeft_CornerHUD.transform.position = topLeft;
    //    playerObjectInteraction.UpperRight_CornerHUD.transform.position = topRight;
    //    playerObjectInteraction.LowerLeft_CornerHUD.transform.position = bottomLeft;
    //    playerObjectInteraction.LowerRight_CornerHUD.transform.position = bottomRight;


        playerObjectInteraction.ScanInfo_HUD.transform.position = bounds.max ;
        playerObjectInteraction.ScanInfo_text.text = "<color=green>// " + playerObjectInteraction.objectCurrentlyScanning.GetComponent<Note>().note;
    }

    void HideTickers(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
  //      playerObjectInteraction.UpperLeft_CornerHUD.SetActive(false);
  //      playerObjectInteraction.UpperRight_CornerHUD.SetActive(false);
  //      playerObjectInteraction.LowerLeft_CornerHUD.SetActive(false);
  //      playerObjectInteraction.LowerRight_CornerHUD.SetActive(false);
        playerObjectInteraction.ScanInfo_HUD.SetActive(false);
    }



}
