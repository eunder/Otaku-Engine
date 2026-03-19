using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEventComponent : MonoBehaviour
{

    public IEnumerator AddDialogue()
    {
        yield return new WaitForSeconds(0);
        DialogueBoxStateMachine.Instance.AddDialogue(GetComponent<DialogueContentObject>());
    }

    public IEnumerator AddDialogue_FrontOfQue()
    {
        yield return new WaitForSeconds(0);
        DialogueBoxStateMachine.Instance.AddDialogue_FrontOfQue(GetComponent<DialogueContentObject>());
    }
}
