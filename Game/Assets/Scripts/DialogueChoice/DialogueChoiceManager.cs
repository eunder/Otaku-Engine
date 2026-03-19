using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueChoiceManager : MonoBehaviour
{
    private static DialogueChoiceManager _instance;
    public static DialogueChoiceManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }






    public GameObject StateObject;

    public List<GameObject> liftOfDialogueChoiceBoxes = new List<GameObject>();

    public int currentIndex = 0;



    public GameObject dialogueChoice_Canvas;

    public Scrollbar scrollbar;


    public bool dialogueChoiceBoxIsActive = false;

    //timer
    public GameObject countDownTimerUIElement;
    public float maxTime = 15.0f;
    private float currentTime;
    public Image countDownBar_Image;
    public Color startColor = Color.green;
    public Color endColor = Color.red;


    //auto positioning
    public float currentYPos = 0;
    public float YAnchoredOffset = 0; //whole window height / 2
    public float spacingOffset = 0; //group component spacing + element height




    public Transform dialogueChoiceContainer;
    public GameObject dialogueChoiceObjectPrefab;
    public void CreateListOfDialogeChoicesFromStateObject(GameObject stateObject)
    {   
        //return if there are no options
        if(stateObject.GetComponent<State>().states.Count <= 0)
        {
            return;
        }

        //set important vars
        StateObject = stateObject;
        maxTime = stateObject.GetComponent<State>().timeUntilChoiceBoxCloses;

        //disable player movement
        PlayerMovementBasic.Instance.enabled = false;

        // if infinite time... hide time bar
        if(maxTime == 0)
        {
            countDownTimerUIElement.SetActive(false);
        }
        else
        {
            countDownTimerUIElement.SetActive(true); 
        }


        //show dialogue picking screen
        dialogueChoice_Canvas.SetActive(true);

        //clear list
        foreach(GameObject obj in liftOfDialogueChoiceBoxes)
        {
            Destroy(obj);
        }
        liftOfDialogueChoiceBoxes.Clear();


        //fill list
        foreach(string state in StateObject.GetComponent<State>().states)
        {
            GameObject dialogueChoice = GameObject.Instantiate(dialogueChoiceObjectPrefab, Vector3.zero, Quaternion.identity);
            dialogueChoice.transform.SetParent(dialogueChoiceContainer);
            dialogueChoice.transform.localScale = new Vector3(1,1,1);
            dialogueChoice.GetComponentInChildren<TextMeshProUGUI>().text = state;
            liftOfDialogueChoiceBoxes.Add(dialogueChoice);
        }



        //start timer
        currentTime = maxTime;
        countDownBar_Image.color = startColor;
        dialogueChoiceBoxIsActive = true;

        currentIndex = 0;
        SetAllChoicesToTransparentExceptCurrentOne();
    }

    public void DisablePlayer()
    {

    }


    void Update()
    {

        if(dialogueChoiceBoxIsActive)
        {
        
        //run if there is no infinite time
        if(maxTime != 0)
        {

            //Timer
            if(currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                
                float normalizedTime = currentTime / maxTime;

                countDownTimerUIElement.SetActive(true);
                //color bar: green to red
                countDownBar_Image.color = Color.Lerp(endColor, startColor, normalizedTime);


                //shrink bar: 1 to 0
                countDownBar_Image.transform.localScale = new Vector3(Mathf.Lerp(0, 1, normalizedTime) , 1,1);
            }
            else
            {
                TriggerTimeOutEvent();
                dialogueChoiceBoxIsActive = false;
            }
        }


        if(Input.GetKeyDown(KeyCode.Space))
        {
            TriggerCurrentIndexSelected();
        } 


                //move up and down
        if(Input.GetKeyDown(KeyCode.W))
        {
        currentIndex = Mathf.Max(0, currentIndex - 1);        
        SetAllChoicesToTransparentExceptCurrentOne();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
        currentIndex = Mathf.Min(liftOfDialogueChoiceBoxes.Count - 1, currentIndex + 1);
        SetAllChoicesToTransparentExceptCurrentOne();
        }



        //auto positioning towards selected element

        float YpositionOfSelectedElement = (currentIndex * spacingOffset) - YAnchoredOffset;

        currentYPos = Mathf.Lerp(currentYPos, YpositionOfSelectedElement, 15.0f * Time.deltaTime);


        dialogueChoiceContainer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, currentYPos);
        

        }
        }

    public void SetAllChoicesToTransparentExceptCurrentOne()
    {
        foreach(GameObject obj in liftOfDialogueChoiceBoxes)
        {
            obj.GetComponent<CanvasGroup>().alpha = 0.2f;
        }
        liftOfDialogueChoiceBoxes[currentIndex].GetComponent<CanvasGroup>().alpha = 1.0f;
    }


    public void TriggerCurrentIndexSelected()
    {
        EventActionManager.Instance.TryPlayEvent(StateObject, "OnAnyDialogueChoicePicked");


        //dont set player movement if they are currently in a poster-viewing state
        if(PlayerObjectInteractionStateMachine.Instance.currentState != PlayerObjectInteractionStateMachine.Instance.PlayerObjectInteractionStateViewingFrame)
        {
        //enable player movement
        PlayerMovementBasic.Instance.enabled = true;
        PlayerMovementBasic.Instance.jumpInputPressThreshold = 0;
        PlayerMovementTypeKeySwitcher.Instance.enabled = true;
        }


        //trigger the state from the state machine
        StateObject.GetComponent<State>().currentState = StateObject.GetComponent<State>().states[currentIndex];
        EventActionManager.Instance.TryPlayEvent_DialogueBox(StateObject, "OnDialogueChoice");

        dialogueChoice_Canvas.SetActive(false);
        dialogueChoiceBoxIsActive = false;
    }

    public void TriggerTimeOutEvent()
    {
        //dont set player movement if they are currently in a poster-viewing state
        if(PlayerObjectInteractionStateMachine.Instance.currentState != PlayerObjectInteractionStateMachine.Instance.PlayerObjectInteractionStateViewingFrame)
        {
        //enable player movement
        PlayerMovementBasic.Instance.enabled = true;
        PlayerMovementBasic.Instance.jumpInputPressThreshold = 0;
        PlayerMovementTypeKeySwitcher.Instance.enabled = true;
        }

        EventActionManager.Instance.TryPlayEvent(StateObject, "OnDialogueChoiceTimeOut");

        dialogueChoice_Canvas.SetActive(false);
        dialogueChoiceBoxIsActive = false;


    }


}