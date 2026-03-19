using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using TMPro;
public class WheelPickerHandler : MonoBehaviour
{
    private static WheelPickerHandler _instance;
    public static WheelPickerHandler Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    public GameObject InitialListObject;
    public GameObject editMode_PrefabList;
    public GameObject playMode_PrefabList;

    public GameObject objectToInstantiate;
    public WheelPickerCenterLineDrawer wheelPickerLineDrawer;
    public int numberOfElements = 4;
    public float radius = 150f;
    public List<GameObject> currentElements = new List<GameObject>();
    public Transform elementGroupParent;

    public  Animator wheelAnimator;

    public AnimationCurve animationCurve_Fade;
    public RawImage rawImage_Fade;

    public SimpleSmoothMouseLook mouseLooker;

    //to prevent player from opening wheel
    public ItemEditStateMachine itemEditStateMachine;
    public PlayerObjectInteractionStateMachine playerObjectInteractionStateMachine;
    public GridBuilderStateMachine gridBuilderStateMachine;

    public CanvasGroup canvasGroup;
    private float toolTipPopUp_Time = 0.0f;

    public TextMeshProUGUI toolTipObjectName_Text;
    public TextMeshProUGUI toolTipObjectTips_Text;

    public void BuildWheelPicker()
    {
        for (int i = 0; i < numberOfElements; i++)
        {
            float angle = i * Mathf.PI*2f / numberOfElements;
            Vector2 newPos = new Vector2(0,0) + new Vector2(Mathf.Sin(angle)*radius, Mathf.Cos(angle)*radius);
            GameObject go = Instantiate(objectToInstantiate, newPos, Quaternion.identity);
            go.transform.parent = elementGroupParent;
            go.GetComponent<RectTransform>().anchoredPosition = newPos;
            
            currentElements.Add(go);
        }

            //sets ALL the elements' isHover value to false
            foreach(GameObject elementObj in currentElements)
            {
            elementObj.GetComponent<WheelPickerElementGrowOnHover>().closetObj = false;
            }
    }
    public bool wheelPickerIsOpen = false;
    public GameObject Objectlist;
    public void BuildWheelPickerFromList()
    {
                                ClearWheelPicker();

        Objectlist = Instantiate(Objectlist, new Vector3(0f,0f,0f), Quaternion.identity);

        RectTransform[] allChildren = Objectlist.GetComponentsInChildren<RectTransform>();
        for(int i = 0; i < allChildren.Length; i++)
        {
            float angle = i * Mathf.PI*2f / allChildren.Length;
            Vector2 newPos = new Vector2(0,0) + new Vector2(Mathf.Sin(angle)*radius, Mathf.Cos(angle)*radius);
            allChildren[i].transform.SetParent(elementGroupParent);
            allChildren[i].GetComponent<RectTransform>().anchoredPosition = newPos;
            currentElements.Add(allChildren[i].gameObject); 
        }



            //sets ALL the elements' isHover value to false
            foreach(GameObject elementObj in currentElements)
            {
            elementObj.GetComponent<WheelPickerElementGrowOnHover>().closetObj = false;
            }

            //parent of list of button gameobjects(the buttons were moved to another parent)
            Destroy(Objectlist);
    }


    //these two functions are mostly used to auto popoulate the wheel on certain events (state object logic entities)...
    public void AddToWheelPickerList(GameObject elementToAdd)
    {
        currentElements.Add(elementToAdd);
    }


    public void RefreshWheelPickerList()
    {
        for(int i = 0; i < currentElements.Count; i++)
        {
            float angle = i * Mathf.PI*2f / currentElements.Count;
            Vector2 newPos = new Vector2(0,0) + new Vector2(Mathf.Sin(angle)*radius, Mathf.Cos(angle)*radius);
            currentElements[i].transform.SetParent(elementGroupParent);
            currentElements[i].GetComponent<RectTransform>().anchoredPosition = newPos;
        }


              //sets ALL the elements' isHover value to false
            foreach(GameObject elementObj in currentElements)
            {
            elementObj.GetComponent<WheelPickerElementGrowOnHover>().closetObj = false;
            }
    }



    public void ClearWheelPicker()
    {

            foreach(GameObject elementObj in currentElements)
            {
            Destroy(elementObj);
            } 
            currentElements.Clear();
    }

    public void CloseWheelPicker()
    {
            ClearWheelPicker();
            Cursor.lockState = CursorLockMode.Locked;
            elementGroupParent.gameObject.SetActive(false);
            mouseLooker.wheelPickerIsTurnedOn = false;
            Objectlist = InitialListObject;
            wheelPickerIsOpen = false;
           // playerObjectInteractionStateMachine.enabled = true;
         //   toolBarCANVAS.SetActive(false);
    }


    public void BringUpWheelMenu()
    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        elementGroupParent.gameObject.SetActive(true);
                        wheelAnimator.Play("WheelPicker_Open1", -1, 0f);
                        mouseLooker.wheelPickerIsTurnedOn = true;
                        wheelPickerIsOpen = true;

                        //disable to prevent player interacting with world while in wheel menu
                   //     playerObjectInteractionStateMachine.enabled = false;
    }

            public GameObject closestObj = null;
            public Vector2 currentMousePos;

    
    GameObject toolBarCANVAS;
    void Start()
    {
          toolBarCANVAS = GameObject.Find("CANVAS_TOOLBAR");

          if(EditModeStaticParameter.isInEditMode)
          {
            InitialListObject = editMode_PrefabList;
          }
          else
          {
            InitialListObject = playMode_PrefabList;
          }

          Objectlist = InitialListObject;
    }

    private float time = 0.0f;
    // Update is called once per frame

    public bool didPointerEnterToolBar = false;
    void Update()
    {

            //space bar to bring up wheel
            if(Input.GetKeyDown(KeyCode.Q) && SaveAndLoadLevel.Instance.isLevelLoaded == true)
            {
                if(itemEditStateMachine.currentState == itemEditStateMachine.ItemEditStateMachineStateIdle && playerObjectInteractionStateMachine.currentState == playerObjectInteractionStateMachine.PlayerObjectInteractionStateIdle && gridBuilderStateMachine.currentState == gridBuilderStateMachine.GridBuilderStateIdle && EscapeToggleToolBar.toolBarisOpened == false)
                {
                        BuildWheelPickerFromList();
                        BringUpWheelMenu();
                }
            }
            if(Input.GetKeyUp(KeyCode.Q))
            {
                                if(itemEditStateMachine.currentState == itemEditStateMachine.ItemEditStateMachineStateIdle && playerObjectInteractionStateMachine.currentState == playerObjectInteractionStateMachine.PlayerObjectInteractionStateIdle && gridBuilderStateMachine.currentState == gridBuilderStateMachine.GridBuilderStateIdle && EscapeToggleToolBar.toolBarisOpened == false)
                {
                CloseWheelPicker();
                }
            }

            if(wheelPickerIsOpen == true)
            {
               time += Time.deltaTime;
            }
            else
            {
                time -= Time.deltaTime;
            }

            time = Mathf.Clamp(time, 0.0f, animationCurve_Fade[animationCurve_Fade.length - 1].time);

            rawImage_Fade.color = new Color(rawImage_Fade.color.r, rawImage_Fade.color.g, rawImage_Fade.color.g, animationCurve_Fade.Evaluate(time));
            
            wheelPickerLineDrawer.enabled = true;
            //click and activate event in closest object
            if(closestObj !=null)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    if(closestObj.GetComponent<WheelPickerButtonEventHolder>())
                    {
                    closestObj.GetComponent<WheelPickerButtonEventHolder>().PlayButtonEvent();
                    }
                }

            }


            //layout the elements around circle and stuff

            currentMousePos = Input.mousePosition;
            currentMousePos = new Vector2(currentMousePos.x - Screen.width/2, currentMousePos.y - Screen.height/2);

             closestObj = null;
            float minDist = Mathf.Infinity;

        if(Mathf.Abs(currentMousePos.x) > 10 || Mathf.Abs(currentMousePos.y) > 10)  //to keep the mouse from picking an object as soon as the menu is opened
        {

        //calculates the closest object from mouse cursor
        foreach(GameObject elementObj in currentElements)
        {
           float dist = Vector2.Distance(elementObj.GetComponent<RectTransform>().anchoredPosition, currentMousePos);

             if (dist < minDist)
        {
            //sets ALL the elements' isHover value to false
            foreach(GameObject elementObj2 in currentElements)
            {
            elementObj2.GetComponent<WheelPickerElementGrowOnHover>().closetObj = false;
            }

            closestObj = elementObj;

            //sets the closest element's isHover value to true
            closestObj.GetComponent<WheelPickerElementGrowOnHover>().closetObj = true;
            minDist = dist;
        }
        }
        }
        else
        {
            //sets ALL the elements' isHover value to false
            foreach(GameObject elementObj in currentElements)
            {
            elementObj.GetComponent<WheelPickerElementGrowOnHover>().closetObj = false;
            }
            closestObj = null;
            wheelPickerLineDrawer.enabled = false;
        }

        //if there is no closest object selected, then dont show the boxes
        if(closestObj != null)
        {
            //if the object has no tip componenet, then dont show the boxes
            if(closestObj.GetComponent<ToolTipTextHolder>())
            {
                    //test fill text fields
                    toolTipObjectName_Text.text = closestObj.GetComponent<ToolTipTextHolder>().ObjectName;
                    toolTipObjectTips_Text.text = "Tips <br> ----- <br> <br>";
                    
                    foreach(string tip in closestObj.GetComponent<ToolTipTextHolder>().objectTips)
                    {
                        toolTipObjectTips_Text.text += "* " + tip + "<br> <br>";
                    }

                    toolTipPopUp_Time += Time.deltaTime * 3.0f;
            }
            else
            {
                            toolTipPopUp_Time -= Time.deltaTime * 6.0f;
            }
        }
        else
        {
            toolTipPopUp_Time -= Time.deltaTime * 6.0f;
        }
                        canvasGroup.alpha = toolTipPopUp_Time; 

                            toolTipPopUp_Time = Mathf.Clamp(toolTipPopUp_Time,0.0f, 1.0f);

    }




}
