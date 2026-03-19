
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StringCanvasUpdater : MonoBehaviour
{
    public StringEntity stringEnt;
    public TextMeshProUGUI noteText;

    void CheckIfIsParentedToPointerEntity()
    {    
        if(transform.parent.GetComponent<GlobalParameterPointerEntity>())
        if(transform.parent.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<StringEntity>())
        stringEnt = transform.parent.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<StringEntity>();
    }


    // Update is called once per frame
    void Update()
    {
        if(stringEnt)
        {
            noteText.text = stringEnt.currentString;
        }
    }
}
