using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomeItemType : MonoBehaviour
{
    //Define Enum
     public enum TypeOfItem{empty, poster, video, screen, door};
     
     //This is what you need to show in the inspector.
     public TypeOfItem itemType;
}
