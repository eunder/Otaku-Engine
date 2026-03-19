using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepositionPlayer : MonoBehaviour
{
    public Transform player;
    
    public void Reposition()
    {
        player.position = transform.position;
    }
}
