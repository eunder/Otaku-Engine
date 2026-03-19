using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NoteWorldCanvasUpdater : MonoBehaviour
{
    public Note noteClass;
    public TextMeshProUGUI noteText;

    // Update is called once per frame
    void Update()
    {

            if(transform.parent.GetComponent<GlobalParameterPointerEntity>())
            {
                noteText.text = "// " + transform.parent.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<Note>().note;
            }
          
          /*
            else if(transform.parent.GetComponent<DialogueContentObject>())
            {
                if(string.IsNullOrWhiteSpace(transform.parent.GetComponent<DialogueContentObject>().dialogue))
                {
                    noteText.text = "";
                }
                else
                {
                    noteText.text = "<color=yellow> ''" + transform.parent.GetComponent<DialogueContentObject>().dialogue + "''</color>";
                }
            }
            */

            else
            {
                noteText.text = "// " + noteClass.note;

                if(string.IsNullOrWhiteSpace(noteClass.note))
                {
                    noteText.text = "";
                }
            }
                

                //if there is any comment on the thing... use that instead...
                if(string.IsNullOrWhiteSpace(noteClass.note) == false)
                {
                    noteText.text = "// " + noteClass.note;
                }
            

    }
}
