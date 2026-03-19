using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeObjectList : MonoBehaviour
{
    public  CustomeItem[] allItems;
    void Start()
    {
    allItems = UnityEngine.Object.FindObjectsOfType<CustomeItem>();
    }

}
