using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{

    //IMORTANT NOTE!!! STATES WERE CHANGED INTO "DialogueChoice"s... keep this in mind when working with the code

    public List<string> states = new List<string>();
    public string currentState; 

    public float timeUntilChoiceBoxCloses = 0f;
}
