using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsScannable : MonoBehaviour
{
    public bool isScannable = false;


        public void OnScanned()
    {
        EventActionManager.Instance.TryPlayEvent(transform.gameObject, "OnScanned");
    }
}
