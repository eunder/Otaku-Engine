using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PosterEventComponent : MonoBehaviour
{

    //The way poster media setting/resetting works:
    //OnSetPosterMedia will change the material and image(vector2d) variables
    //ResetPosterMedia will change these parameters back (using currentMaterial and image_original)


    public IEnumerator SwitchPosterMedia(GameObject poster)
    {
        yield return new WaitForSeconds(0);
    /*    
        GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial = GameObject.Find(posterID).GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial;   
        GetComponent<PosterMeshCreator>().urlFilePath_eventModified = GameObject.Find(posterID).GetComponent<PosterMeshCreator>().urlFilePath;

        GetComponent<PosterMeshCreator>().image = GameObject.Find(posterID).GetComponent<PosterMeshCreator>().image;

        //trigger the delegate event (used for stencil depth layers)
        GetComponent<PosterMeshCreator>().TriggerOnSuccesfulMediaChangeEvent();
*/

        ResetMedia();


        //PAUSE THE CURRENT POSTER'S GIF COMPONENT!

        if(GetComponent<PosterGifPlayer>())
        {
            GetComponent<PosterGifPlayer>().enabled = false;
        }

        if(poster.GetComponent<PosterMeshCreator>().urlFilePath.Contains(".jpg") || 
         poster.GetComponent<PosterMeshCreator>().urlFilePath.Contains(".jpeg") || 
        poster.GetComponent<PosterMeshCreator>().urlFilePath.Contains(".png") || 
        poster.GetComponent<PosterMeshCreator>().urlFilePath.Contains(".PNG") ||
        poster.GetComponent<PosterMeshCreator>().urlFilePath.Contains(".mp4") ||
         poster.GetComponent<PosterMeshCreator>().urlFilePath.Contains(".webm") ||
          poster.GetComponent<PosterMeshCreator>().urlFilePath.Contains(".mkv"))
        {
          GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial.mainTexture = poster.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial.mainTexture;
        }


        //gif stuff (gifs are annoying to work with atm...)
        if(poster.GetComponent<PosterMeshCreator>().urlFilePath.Contains(".gif"))
        {
            poster.GetComponent<PosterGifPlayer>().posterMaterial = GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial;
            GetComponent<PosterMeshCreator>().gifPostersUsedForMediaSwitching.Add(poster);
        }







    }
 

    
    public IEnumerator SaveMediaOnPlayerPC()
    {
        yield return new WaitForSeconds(0);

        
        if(!Directory.Exists(Application.persistentDataPath + "/SavedMedia/"))
        {    
            Directory.CreateDirectory(Application.persistentDataPath + "/SavedMedia/");
        }
        if(GetComponent<PosterMeshCreator>().isVideo)
        {
            GetComponent<PosterMeshCreator>().DownloadVideo(Application.persistentDataPath + "/SavedMedia/"); 
        }
        else
        {
            GetComponent<PosterMeshCreator>().DownloadImage(Application.persistentDataPath + "/SavedMedia/");
        }
    }









    public void ResetMedia()
    {
        GetComponent<PosterMeshCreator>().ResetMedia();
    }
}
