using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSceneStartSpawnRepositionPlayer : MonoBehaviour
{
    public Transform playerObject;
    public Transform spawnObject;
    public SimpleSmoothMouseLook mouseLooker;
    void Start()
    {
        playerObject.position = spawnObject.position + new Vector3(0.0f,0.768f,0.0f);
        playerObject.rotation = spawnObject.rotation;
                mouseLooker.enabled = true;

    }
}
