using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Debug_Overlay : MonoBehaviour
{

    public PlayerObjectInteractionStateMachine objectInteractionStateMachine;
    public TextMeshProUGUI debugList;
    
    void Update()
    {
        
        debugList.text = 
        "ProjectManager.currentOpenedProjectPath: " + "\n" + ProjectManager.currentOpenedProjectPath + "\n \n" +
        "currentPathWorkingWith: " + "\n" + ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith + "\n \n" +  
        "pathToSaveZipAs: " + "\n" + ZipFileHandler_GlobalStaticInfo.pathToSaveZipAs + "\n \n" +  
        "currentPasswordWorkingWith: " + "\n" + ZipFileHandler_GlobalStaticInfo.currentPasswordWorkingWith + "\n \n" +  
        "currentUnzippedTempDirectory: " + "\n" + ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory + "\n \n" +  
        "GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp: "  + "\n" + GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp + "\n \n";


                             

    }
}
