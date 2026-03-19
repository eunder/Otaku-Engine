using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public interface IAIWifeState
    {
        IAIWifeState DoState(AIWifeStateMachine AIWife);
    }