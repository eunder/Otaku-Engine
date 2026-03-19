using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosterFrameList : MonoBehaviour
{
    public List<GameObject> posterFrameList = new List<GameObject>();


    public void AssignWhichFrameIsLast()
    {
            if(posterFrameList.Count >= 1)
    {
        for(int i = 0; i < posterFrameList.Count - 1 ;i++)
        {
            posterFrameList[i].GetComponent<PosterMeshCreator_BorderFrame>().isLastFrame = false;
        }
            posterFrameList[posterFrameList.Count - 1].GetComponent<PosterMeshCreator_BorderFrame>().isLastFrame = true;
    }       
    }

    public void AssignWhichMeshesFramesDrawAround()
    {
    if(posterFrameList.Count >= 1)
    {
          for(int i = posterFrameList.Count - 1; i > 0; i--)
        {  
        posterFrameList[i].GetComponent<PosterMeshCreator_BorderFrame>().meshOfObjectToDrawFrameAround = posterFrameList[i-1].GetComponent<MeshFilter>();
        }
        

        posterFrameList[0].GetComponent<PosterMeshCreator_BorderFrame>().meshOfObjectToDrawFrameAround = GetComponent<MeshFilter>();
    }
       // posterFrameList[1].GetComponent<PosterMeshCreator_BorderFrame>().meshOfObjectToDrawFrameAround = posterFrameList[0].GetComponent<MeshFilter>();

/*
          for(int i = 1; i <= posterFrameList.Count; i++)
        {
        posterFrameList[i].GetComponent<PosterMeshCreator_BorderFrame>().meshOfObjectToDrawFrameAround = posterFrameList[i-1].GetComponent<MeshFilter>();
        }
*/


    }
}
