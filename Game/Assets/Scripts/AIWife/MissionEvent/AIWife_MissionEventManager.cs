using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SFB;

public class AIWife_MissionEventManager : MonoBehaviour
{


    public DialogueBoxStateMachine dialogueBoxStateMachine;
    public DialogueContentObject mapChooseCancel_dialogueObject;
    public DialogueContentObject playerCloseUploadWindow_dialogueObject;
    public DialogueContentObject playerMapChoosen_dialogueObject;
    public DialogueContentObject missionComplete_dialogueObject;

    public DialogueContentObject startMissionSearch_dialogueObject;
    public DialogueContentObject startMissionSearch2_dialogueObject;
    public DialogueContentObject startMissionSearchCompletePromptNameResult_dialogueObject;
    public DialogueContentObject startMissionSearchCompletePromptDialogueEnd_dialogueObject;


    public List<string> listOfPrompts = new List<string>();

    //public SaveAndLoadGameStory saveAndLoadManager;

    public GameObject RecieveMission_MenuObject;
    public GameObject SubmitMission_MenuObject;

    public string missionStatus;

    public void Start()
    {
        StartCoroutine(CheckMissionStatus());
    }

    IEnumerator CheckMissionStatus()
    {
        string json;
        //check if file exists, if not, create it
        if (!System.IO.File.Exists(Application.persistentDataPath+"/save.json"))
        {
            /*
            json = JsonUtility.ToJson(saveAndLoadManager.gameProgressData);
            File.WriteAllText(Application.persistentDataPath+"/save.json", json);
            */
        }
        yield return new WaitForSeconds(0.10f); //not very safe, fix later

        //load game progress
        json = File.ReadAllText(Application.persistentDataPath+"/save.json");
        //saveAndLoadManager.gameProgressData = JsonUtility.FromJson<SaveAndLoadGameStory.GameProgressData>(json);
        
        yield return new WaitForSeconds(0.25f); //not very safe, fix later

        //asign prompt
        //missionStatus = saveAndLoadManager.gameProgressData.missionPrompt;

        //enable the appropriate menu object

        if(string.IsNullOrWhiteSpace(missionStatus))
        {
            RecieveMission_MenuObject.SetActive(true);
        }
        else
        {
            SubmitMission_MenuObject.SetActive(true);
        }

    }

    public void AssignNextMissionPrompt()
    {
    //    saveAndLoadManager.SetMissionPrompt(listOfPrompts[Random.Range(0, listOfPrompts.Count)]);
    }

    public void StartMissionAssigmentSearch()
    {
        dialogueBoxStateMachine.AddDialogue(startMissionSearch_dialogueObject);
        dialogueBoxStateMachine.AddDialogue(startMissionSearch2_dialogueObject);
    }

    public void MissionSearchComplete()
    {

                RecieveMission_MenuObject.SetActive(false);

                string prompt = listOfPrompts[Random.Range(0, listOfPrompts.Count)];
    //            saveAndLoadManager.SetMissionPrompt(prompt);

                startMissionSearchCompletePromptNameResult_dialogueObject.dialogue = "<color=green><size=140%> " + prompt;
                dialogueBoxStateMachine.AddDialogue(startMissionSearchCompletePromptNameResult_dialogueObject);
                dialogueBoxStateMachine.AddDialogue(startMissionSearchCompletePromptDialogueEnd_dialogueObject);
    }

    public void EraseMissionPrompt()
    {
     //   saveAndLoadManager.SetMissionPrompt("");
    }

    public void MissionComplete()
    {
                    EraseMissionPrompt();
                    SubmitMission_MenuObject.SetActive(false);
                    dialogueBoxStateMachine.AddDialogue(missionComplete_dialogueObject);
                    GameObject.Find("AcivateToolbarKey").GetComponent<EscapeToggleToolBar>().CloseToolBarIfOpen();
    }


    public void OpenMapToSubmit()
    {
        Cursor.lockState = CursorLockMode.None;
        var paths = StandaloneFileBrowser.OpenFilePanel("Commision (Submit)", Application.persistentDataPath + "/MyMaps", "json", false);
        if (paths.Length > 0) {

            //dialogue when succesfully retrieved a json path string
            string fileName = paths[0].Split('\\')[paths[0].Split('\\').Length - 1]; // for local files
            playerMapChoosen_dialogueObject.dialogue = "So you want to submit <color=red>" + fileName + "<color=white><size=70%> Remember the prompt is: <color=green>" + missionStatus; 
            dialogueBoxStateMachine.AddDialogue(playerMapChoosen_dialogueObject);

            //fill the url field(input field) and upload path(string)
            GameObject.Find("CANVAS_TOOLBAR_PARENTFINDER").transform.GetChild(0).transform.Find("SubmitMissionUploadManager").GetComponent<AIWife_MissionSubmitUploadManager>().mapPath_InputField.text = paths[0];
            GameObject.Find("CANVAS_TOOLBAR_PARENTFINDER").transform.GetChild(0).transform.Find("SubmitMissionUploadManager").GetComponent<AIWife_MissionSubmitUploadManager>().mapPathToUpload = paths[0];
            GameObject.Find("CANVAS_TOOLBAR_PARENTFINDER").transform.GetChild(0).transform.Find("SubmitMissionUploadManager").GetComponent<AIWife_MissionSubmitUploadManager>().mapDesc_InputField.text = "Prompt: '" + missionStatus + "'";
        }
        else
        {
            //not ready? remember, the prompt is " "
            mapChooseCancel_dialogueObject.dialogue = "Changed your mind?, remember, the prompt is <color=green> " + missionStatus;
            dialogueBoxStateMachine.AddDialogue(mapChooseCancel_dialogueObject);
        }

    }


    //upload process events...

    public void OnDialogueEnd_StartUploadProccess()
    {
            //Pause as if the player did it
            GameObject.Find("AcivateToolbarKey").GetComponent<EscapeToggleToolBar>().OpenToolBarIfClosed();

            //open the upload submit mission window
            GameObject.Find("CANVAS_TOOLBAR_PARENTFINDER").transform.GetChild(0).transform.Find("WINDOW_SubmitMissionUploadMapWindowParentHolder").GetChild(0).GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();
            }


   public void PlayerCloseWindowDialogue()
    {
        GameObject.Find("AcivateToolbarKey").GetComponent<EscapeToggleToolBar>().CloseToolBarIfOpen();
        dialogueBoxStateMachine.AddDialogue(playerCloseUploadWindow_dialogueObject);
    }



}
