using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.UI;
using TMPro;

public class CreateNewProject : MonoBehaviour
{
    public GameObject Window_EnterProjectName;
    public TMP_InputField projectName_inputField;
    

    public void OnCreateNewProject() 
    {
           Window_EnterProjectName.GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();

    }

    public void OnConfirmNewProjectName()
    {
            Window_EnterProjectName.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();
            ProjectManager.Instance.CreateNewProject(projectName_inputField.text);
    }
}
