using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;
using TMPro;


public class DialogueBoxStateMachine : MonoBehaviour
{

    private static DialogueBoxStateMachine _instance;
    public static DialogueBoxStateMachine Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }




    [SerializeField]
    public string currentStateName;
    public IDialogueBoxStateMachine_Interface currentState;
    public DialogueBoxStateMachineState_Idle DialogueBoxStateMachineStateIdle = new DialogueBoxStateMachineState_Idle();
    public DialogueBoxStateMachineState_DisplayingText DialogueBoxStateMachineStateDisplayingText = new DialogueBoxStateMachineState_DisplayingText();
    public DialogueBoxStateMachineState_DisplayingTextEnd DialogueBoxStateMachineStateDisplayingTextEnd = new DialogueBoxStateMachineState_DisplayingTextEnd();

    public GameObject Canvas_dialogueBox;
    public GameObject endSentenceTicker;

    public Animator anim;

    public List<string> sentencesToDisplay = new List<string>();
    public List<DialogueContentObject> dialogueObjectList = new List<DialogueContentObject>();

    public int currentSentenceIndex = 0;

    public bool newStateOnceEvent = false;

    public TextMeshProUGUI dialogueBoxTextUI;
    public float textSpeed = 0.1f;
    public float textSpeed_period = 0.5f;
    
    public AudioSource audioSource_Voice;
    public AudioClip[] voices;
    public float voicePitchLow = 1.0f;
    public float voicePitchHigh = 2.0f;
    public Transform mouthQuad;
    public Transform npcBody; //for slight talking movement
    public float mouthOpenAndCloseSpeed = 0.1f;
    public int sentencesLeft = 0;

    public AudioSource dialogueEnd_audioSource;
    public AudioSource dialogueEnd_audioSource_Last;

    public Animator dialogueBox_anim;

    public AnimationCurve testAnimeEaseCurve;


    void OnEnable()
    {
        currentState = DialogueBoxStateMachineStateIdle;
    }
    void Update()
    {

        currentState = currentState.DoState(this);

        if(currentStateName != currentState.ToString()) // makes sure the enter event happens by checking if the state name has changed
        {
            newStateOnceEvent = false;
        }

        currentStateName = currentState.ToString();


    }
 
    public bool addedImportantDialogueEvent = false;

    //adds it to the que... (back of the list)
    public void AddDialogue(DialogueContentObject dialogueObject)
    {
        //check if anything in the list was a preview object, it if was... destory it
        foreach(DialogueContentObject d in dialogueObjectList)
        {
            if(d.gameObject.name == "PreviewDialogue")
            {
                Destroy(d.gameObject);
            }
        }


        if(dialogueObject.clearPreviousDialogue)
        {
            dialogueObjectList.Clear();
            currentSentenceIndex = 0;
            addedImportantDialogueEvent = true;
        }
        dialogueObjectList.Add(dialogueObject);
    }

    
    public void AddDialogue_FrontOfQue(DialogueContentObject dialogueObject)
    {
        //check if anything in the list was a preview object, it if was... destory it
        foreach(DialogueContentObject d in dialogueObjectList)
        {
            if(d.gameObject.name == "PreviewDialogue")
            {
                Destroy(d.gameObject);
            }
        }

        dialogueObjectList.Insert(0, dialogueObject);
        addedImportantDialogueEvent = true;
    }


    public void ClearAllCurrentDialogue()
    {
            //check if anything in the list was a preview object, it if was... destory it
            foreach(DialogueContentObject d in dialogueObjectList)
            {
                if(d.gameObject.name == "PreviewDialogue")
                {
                    Destroy(d.gameObject);
                }
            }



            dialogueObjectList.Clear();
            currentSentenceIndex = 0;
            addedImportantDialogueEvent = true;
    }

}
