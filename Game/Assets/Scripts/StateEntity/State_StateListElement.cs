using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_StateListElement : MonoBehaviour
{
    public GameObject mainElementObjectToSend;

    public void DeleteState()
    {
        ItemEditStateMachine.Instance.DeleteStateFromList(mainElementObjectToSend);    
    }


    public void MoveUpOnList()
    {
        ItemEditStateMachine.Instance.MoveStateLayerUp(mainElementObjectToSend);    
    }
    public void MoveDownOnList()
    {
        ItemEditStateMachine.Instance.MoveStateLayerDown(mainElementObjectToSend);    
    }

    public void OnFinishEditing()
    {
        ItemEditStateMachine.Instance.UpdateStateString(mainElementObjectToSend);    
    }
}
