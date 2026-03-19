using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorkshopQueryOntoBlock_PressedToUpdate : MonoBehaviour
{
 public void PressedToUpdate()
    {
        
        GameObject.Find("WINDOW_UpdateMapMyWorkshopWindowPicker").GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();
        GameObject.Find("WINDOW_UploadMapWindowParentHolder").transform.GetChild(0).GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();

        try
        {
                //write thumbnail to streaming assests to use as updater path later
                byte[] bytes = GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().imageToTestFrom.EncodeToPNG();
                System.IO.File.WriteAllBytes(Application.dataPath + "/StreamingAssets/" + "update_picture.png", bytes);
                GameObject.Find("UploadManager").GetComponent<UploadManager>().UpdateFields(GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo, true);
        }
        catch
        {
            Debug.Log("ERROR, was texture null?");
            GameObject.Find("UploadManager").GetComponent<UploadManager>().UpdateFields(GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo, false);
        }
    }
}
