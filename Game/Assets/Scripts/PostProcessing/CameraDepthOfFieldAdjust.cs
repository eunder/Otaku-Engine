using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDepthOfFieldAdjust : MonoBehaviour
{
    private static CameraDepthOfFieldAdjust _instance;
    public static CameraDepthOfFieldAdjust Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    public float maxFocusDistance;
    public float hitDistance;

    bool isHit;



    bool useXrayLogic = false;
    GameObject closestGameObjectThatIsScannable;
    RaycastHit[] allHits;


    void Update()
    {
        //if scanner is out... then use the same scanner logic for figuring out DoF. (ignores faces that have an xray _size value greater than 0) 
        if(PlayerObjectInteractionStateMachine.Instance.currentState == PlayerObjectInteractionStateMachine.Instance.PlayerObjectInteractionStateEquippedScanner)
        {

                    allHits = Physics.RaycastAll(transform.position, transform.forward, XrayRaycast.Instance.maxDistance, PlayerObjectInteractionStateMachine.Instance.holding_collidableLayers_layerMask);
            
                    System.Array.Sort(allHits, (x,y) => x.distance.CompareTo(y.distance));

                    //only allow scanning if an object that is scannable and not obstructed was found.
                    if(GetPointOfClosestScannableGameObject() != null)
                    {
                        hitDistance = Vector3.Distance(transform.position, GetPointOfClosestScannableGameObject());
                    }

        }
        else
        {
            Ray raycast;
            RaycastHit hit;

            raycast = new Ray(transform.position, transform.forward * maxFocusDistance);
            isHit = false;

            if(Physics.Raycast(raycast, out hit, maxFocusDistance, PlayerObjectInteractionStateMachine.Instance.holding_collidableLayers_layerMask))
            {
                isHit = true;
                hitDistance = Vector3.Distance(transform.position, hit.point);
            }
            else
            {
                isHit = false;
            }

        }


        SetFocus();
    }


        Vector3 GetPointOfClosestScannableGameObject()
        {
            for (int i = 0; i < allHits.Length; i++)
            {
                if(allHits[i].transform.GetComponent<IsScannable>() && allHits[i].transform.GetComponent<IsScannable>().isScannable == true)
                {
                return allHits[i].point;
                }

                if(allHits[i].transform.GetComponent<Block>())
                {
                    if(allHits[i].transform.GetComponent<BlockFaceTextureUVProperties>().CheckIfFaceHasXrayPropertiesBasedOnIndex(allHits[i].triangleIndex) == false)
                    return allHits[i].point;
                }

                if(allHits[i].transform.GetComponent<PosterMeshCreator>())
                {
                    if(allHits[i].transform.GetComponent<PosterMeshCreator>().CheckIfPosterMaterialHasXrayProperties() == false)
                    return allHits[i].point;
                }

            }
                    return new Vector3(0,0,0);

        }







    void SetFocus()
    {
        if(isHit)
        {
            PostProcessingManager.Instance.depthOfField.focusDistance.value = Mathf.Lerp(PostProcessingManager.Instance.depthOfField.focusDistance.value, hitDistance, Time.deltaTime * 8.0f);
        }
        else
        {
            PostProcessingManager.Instance.depthOfField.focusDistance.value = Mathf.Lerp(PostProcessingManager.Instance.depthOfField.focusDistance.value, maxFocusDistance / 5, Time.deltaTime * 8.0f);
        }
    }

}
