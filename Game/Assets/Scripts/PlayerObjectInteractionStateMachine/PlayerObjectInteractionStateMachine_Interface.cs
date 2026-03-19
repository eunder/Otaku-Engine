using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public interface IPlayerObjectInteractionState
    {
        IPlayerObjectInteractionState DoState(PlayerObjectInteractionStateMachine playerObjectInteraction);
    }