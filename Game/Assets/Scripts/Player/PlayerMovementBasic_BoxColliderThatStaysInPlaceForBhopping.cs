using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBasic_BoxColliderThatStaysInPlaceForBhopping : MonoBehaviour
{
    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
