using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBasic_CrushDetectionCapsule : MonoBehaviour
{
        public CapsuleCollider capsuleCollider;

        public float shrinkMultiplier = 1; // how small should the capsule be compared to player capsule?


        void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == 14)
            {
                MissionStatusManager.Instance.GameOver_Crushed();
            }
            
        }

}
