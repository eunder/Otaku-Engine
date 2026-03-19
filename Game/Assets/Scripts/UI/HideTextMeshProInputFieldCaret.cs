using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HideTextMeshProInputFieldCaret : MonoBehaviour
{
    // Start is called before the first frame update
    void Update()
    {
        GetComponentInChildren<TMP_SelectionCaret>().raycastTarget  = false;
    }
}
