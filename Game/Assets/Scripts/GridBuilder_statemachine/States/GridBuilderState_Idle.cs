using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilderState_Idle : IGridBuilderState 
{
    public IGridBuilderState DoState(GridBuilderStateMachine gridBuilder)
    {
        if(gridBuilder.newStateOnceEvent == false)
        {
            OnEnter(gridBuilder);
            gridBuilder.newStateOnceEvent = true;
        }


        OnStay(gridBuilder);

            if(gridBuilder.i == 1)
            {
                OnExit(gridBuilder);
                return gridBuilder.GridBuilderStateCubeCreator;
            }
           
        return gridBuilder.GridBuilderStateIdle;
    }


    
        void OnEnter(GridBuilderStateMachine gridBuilder)
    {
        PlayerObjectInteractionStateMachine.Instance.enabled = true;
    }

        void OnStay(GridBuilderStateMachine gridBuilder)
        {

        }

        void OnExit(GridBuilderStateMachine gridBuilder)
        {
            
        }



}
