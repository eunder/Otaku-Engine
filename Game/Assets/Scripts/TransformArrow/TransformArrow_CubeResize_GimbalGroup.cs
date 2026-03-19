using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformArrow_CubeResize_GimbalGroup : MonoBehaviour
{

    public List<GameObject> arrowGroup = new List<GameObject>();

    public GameObject arrowBeingHoevered;

    public void HideAllExceptThisArrow(GameObject arrowobj)
    {
        foreach(GameObject obj in arrowGroup)
        {
                obj.SetActive(false);
        }

            arrowobj.SetActive(true);

    }

    public void UnhideAllArrows()
    {
        foreach(GameObject obj in arrowGroup)
        {
                obj.SetActive(true);
        }
    }
}
