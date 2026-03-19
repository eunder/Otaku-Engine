using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCurrentMapOrProjectAsDefaultOnStart : MonoBehaviour
{
    public void SetCurrentMapOrProjectAsDefault()
    {
        ConfigFileHandler.Instance.SetCurrentMapOrProjectAsDefaultOnStartup();
    }

}
