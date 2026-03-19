using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_StateListElement_DeleteState : MonoBehaviour
{
    public GameObject mainElementObjectToSend;
    public void DeleteState()
    {
        ItemEditStateMachine.Instance.DeleteStateFromList(mainElementObjectToSend);    
    }

}
