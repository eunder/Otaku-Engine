using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Event_SetParameter : MonoBehaviour
{

    public void fadeOutParameter()
    {
        gameObject.GetComponent<Animator>().SetBool("FadeOut", true);
    }

}
