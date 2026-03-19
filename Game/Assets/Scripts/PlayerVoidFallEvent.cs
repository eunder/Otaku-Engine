using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVoidFallEvent : MonoBehaviour
{
    public Transform player;

    // Update is called once per frame
    void Update()
    {
        if(player.position.y < -150f)
        {
            MissionStatusManager.Instance.GameOver_FallVoid();
        }
    }
}
