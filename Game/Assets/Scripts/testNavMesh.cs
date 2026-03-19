using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testNavMesh : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
                    GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(PlayerMovementBasic.Instance.transform.position); // Set the destination to the player's position
    }
}
