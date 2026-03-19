using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEventComponent : MonoBehaviour
{
    public void LoadMap()
    {
        GetComponent<Door>().LoadMap();
    }

/*
    public void LoadContent_Additively()
    {
        StartCoroutine(GetComponent<Door>().LoadContent_Additively());
    }
    

    public void LoadContent_Additively_DoorOrigin()
    {
        StartCoroutine(GetComponent<Door>().LoadContent_Additively_DoorOrigin());
    }
    */
}
