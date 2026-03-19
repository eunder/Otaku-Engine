using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ProjectMapPackFileSystemManager : MonoBehaviour
{

    public static void CreateProjectFiles(string pathToProjectFolder)
    {
        string filePath = pathToProjectFolder + "/Project.json";

        ProjectJsonFile.ProjectData projectData = new ProjectJsonFile.ProjectData();
        string jsonContent = JsonUtility.ToJson(projectData, true);
        File.WriteAllText(filePath, jsonContent);


        //create the "Maps" folder
        System.IO.Directory.CreateDirectory(pathToProjectFolder + "/Maps");
    }


}
