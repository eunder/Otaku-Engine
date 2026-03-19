using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public interface IGridBuilderState
    {
        IGridBuilderState DoState(GridBuilderStateMachine gridBuilder);
    }