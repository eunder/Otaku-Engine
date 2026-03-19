using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public interface IItemEditStateMachine
    {
        IItemEditStateMachine DoState(ItemEditStateMachine itemEditor);
    }