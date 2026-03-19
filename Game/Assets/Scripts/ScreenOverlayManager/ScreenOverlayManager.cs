using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOverlayManager : MonoBehaviour
{
    private static ScreenOverlayManager _instance;
    public static ScreenOverlayManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    public List<GameObject> posterOverlayCurrentList = new List<GameObject>();
    public List<GameObject> posterViewingEmulatorList = new List<GameObject>();
    public List<GameObject> posterViewingEmulatorList_Skybox = new List<GameObject>();


    public GameObject cameraViewerEmulatorPrefab;
    public GameObject cameraViewerEmulatorSkyboxPrefab;

    public void AddPosterToOverlay(GameObject poster, bool skybox_mod)
    {
            posterOverlayCurrentList.Add(poster);



            //Disable colliders of all children of poster (so that stuff like flashlights are possible)
            foreach(Transform child in poster.transform)
            {
                if(child.GetComponent<Collider>())
                {
                    child.GetComponent<Collider>().enabled = false;
                }
            }





            //Disable collider of poster and make it render over everything
            poster.GetComponent<GeneralObjectInfo>().DisableCollider();
            poster.GetComponent<Renderer>().material.SetFloat("_ZTest", 8);
            
            //disable vertex snapping
            poster.GetComponent<Renderer>().material.SetFloat("_DisableVertexSnapping", 1);
            foreach(GameObject frame in poster.GetComponent<PosterFrameList>().posterFrameList)
            {
                frame.GetComponent<Renderer>().materials[0].SetFloat("_DisableVertexSnapping", 1);
                frame.GetComponent<Renderer>().materials[1].SetFloat("_DisableVertexSnapping", 1);
            }
            

            Camera playerCamera = SimpleSmoothMouseLook.Instance.GetComponent<Camera>();

        poster.transform.SetParent(playerCamera.transform);
        Quaternion desiredRotation = Quaternion.Euler(-90f, 0f, 0f);
        poster.transform.localRotation = desiredRotation;



        if(skybox_mod == false)
        {
            //to make sure poster isnt overlayed again (will cause errors if its overlayed again)
            bool posterAlreadyOverlayed = false;
            
            foreach(GameObject obj in posterViewingEmulatorList)
            {
                if(obj.GetComponent<ViewingFrameEmulator>().currentGameObjectLookingAt == poster)
                {
                    posterAlreadyOverlayed = true;
                    break;
                }
            }

            if(posterAlreadyOverlayed == false)
            {
            GameObject cameraEmu = Instantiate(cameraViewerEmulatorPrefab, new Vector3(0,0,0), Quaternion.identity);
            cameraEmu.GetComponent<ViewingFrameEmulator>().InitiateCamera(poster, posterViewingEmulatorList.Count + 1 + 3000); //high render que since its up close
            posterViewingEmulatorList.Add(cameraEmu);
            }

            
            EventActionManager.Instance.TryPlayEvent(poster, "OnViewPoster");
        }
        else
        {
            //to make sure poster isnt overlayed again (will cause errors if its overlayed again)
            bool posterAlreadyOverlayed = false;
            
            foreach(GameObject obj in posterViewingEmulatorList_Skybox)
            {
                if(obj.GetComponent<AlternativeSkyboxManager>().currentGameObjectLookingAt == poster)
                {
                    posterAlreadyOverlayed = true;
                    break;
                }
            }
            
            if(posterAlreadyOverlayed == false)
            {
                GameObject cameraEmu = Instantiate(cameraViewerEmulatorSkyboxPrefab, new Vector3(0,0,0), Quaternion.identity);
                cameraEmu.GetComponent<AlternativeSkyboxManager>().InitiateCamera(poster, posterViewingEmulatorList.Count + 1);
                posterViewingEmulatorList_Skybox.Add(cameraEmu);
            }

        }



        //if the game has atleast one skybox overlay.. set the player camera clear flag to "depth only"
        if(posterViewingEmulatorList_Skybox.Count >= 1)
        SimpleSmoothMouseLook.Instance.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
     


    }

    public void RemovePostersFromOverlay(List<GameObject> list)
    {
            foreach(GameObject poster in list)
            {
                if(poster)
                {
                    //Re-enable colliders of all children of poster
                    foreach(Transform child in poster.transform)
                    {
                        if(child.GetComponent<GeneralObjectInfo>())
                        {
                            child.GetComponent<GeneralObjectInfo>().EnableCollider();
                        }
                    }




                    poster.GetComponent<GeneralObjectInfo>().SetParentAccordingToParentID();
                    poster.GetComponent<GeneralObjectInfo>().ResetPosition();
                    poster.GetComponent<GeneralObjectInfo>().EnableCollider();
                    poster.GetComponent<Renderer>().material.SetFloat("_ZTest", 4);


                    //disable enable snapping for poster and its frames
                    poster.GetComponent<Renderer>().material.SetFloat("_DisableVertexSnapping", 0);
                    foreach(GameObject frame in poster.GetComponent<PosterFrameList>().posterFrameList)
                    {
                        frame.GetComponent<Renderer>().materials[0].SetFloat("_DisableVertexSnapping", 0);
                        frame.GetComponent<Renderer>().materials[1].SetFloat("_DisableVertexSnapping", 0);
                    }




                        //if the poster has depth layers... set back the render que of the depth frames
                        if(poster.GetComponent<PosterDepthLayerList>().posterDepthLayerList.Count >= 1)
                        {
                            foreach(GameObject depthLayer in poster.GetComponent<PosterDepthLayerList>().posterDepthLayerList)
                            {
                                depthLayer.GetComponent<Poster_DepthStencilFrame>().UpdateGeneralValues();
                            }

                        }



                    //remove the emulator objects

                    foreach(GameObject emulator in posterViewingEmulatorList)
                    {
                        if(emulator.GetComponent<ViewingFrameEmulator>().currentGameObjectLookingAt == poster)
                        {
                            emulator.GetComponent<ViewingFrameEmulator>().RemoveCameraEmulator();
                            posterViewingEmulatorList.Remove(emulator);
                            break;
                        }
                    }


                    foreach(GameObject emulator in posterViewingEmulatorList_Skybox)
                    {
                        if(emulator.GetComponent<AlternativeSkyboxManager>().currentGameObjectLookingAt == poster)
                        {
                            emulator.GetComponent<AlternativeSkyboxManager>().RemoveCameraEmulator();
                            posterViewingEmulatorList_Skybox.Remove(emulator);
                            break;
                        }

                    }
                }
            }
        



            //after the left/right posters for the skybox emulator are deleted. reset the stencil id's in the map 
            PosterDepthLayerStencilRefManager.Instance.AssignCorrectStencilRefsToAllPostersInScene();




            if(posterViewingEmulatorList_Skybox.Count <= 0)
            {
                //set the player camera clear flag back to "skybox"
                SimpleSmoothMouseLook.Instance.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
            }


    }
    



}
