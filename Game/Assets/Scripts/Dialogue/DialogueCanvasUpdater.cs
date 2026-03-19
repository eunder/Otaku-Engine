using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueCanvasUpdater : MonoBehaviour
{
    public DialogueContentObject dialogue;
    public TextMeshProUGUI dialogueText;


    // Update is called once per frame
    void Update()
    {
        dialogueText.text = dialogue.dialogue;
    }
}
