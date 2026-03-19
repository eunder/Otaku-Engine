using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class OnPlayerRayInteract : MonoBehaviour
{
    public UnityEvent eventOnInteract;

    public void onPlayerInteract()
    {
    eventOnInteract.Invoke();
    }
}
