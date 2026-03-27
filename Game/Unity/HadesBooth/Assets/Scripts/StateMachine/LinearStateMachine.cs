
using System.Collections.Generic;
using UnityEngine;

/**
 * A basic state machine in which one state will always transition to the next (no branching or looping)
 *
 * States will be run in the order that they are added via AddState
 *
 * Methods:
 * - AddState: adds a State to the end of this state machine
 */
public class LinearStateMachine<TStatus> : State<TStatus, DefaultTransition>
{
    protected List<State<TStatus, DefaultTransition>> stateOrder;
    protected int currentState;
    
    public LinearStateMachine(TStatus status, string id = null) : base(status, id)
    {
        stateOrder = new List<State<TStatus, DefaultTransition>>();
        currentState = 0;
    }
    
    public void AddState(State<TStatus, DefaultTransition> state)
    {
        stateOrder.Add(state);
    }

    public override void Setup()
    {
        base.Setup();
        currentState = 0;
        stateOrder[currentState].Setup();
    }

    protected override DefaultTransition Run()
    {
        DefaultTransition baseTrans = base.Run();
        if (baseTrans != null) return baseTrans;

        DefaultTransition stateTrans = stateOrder[currentState].Update();
        if (stateTrans != null)
        {
            stateOrder[currentState].Cleanup();
            currentState++;
            if (currentState >= stateOrder.Count)
            {
                Debug.Log($"Exiting linear state machine after {stateOrder[currentState - 1]}");
                return DefaultTransition.Default;
            }
            Debug.Log($"Transitioning from {stateOrder[currentState - 1]} to {stateOrder[currentState]}");
            stateOrder[currentState].Setup();
        }

        return null;
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        if (currentState < stateOrder.Count) stateOrder[currentState].LateUpdate();
    }

    public override void Cleanup()
    {
        base.Cleanup();
        if (currentState < stateOrder.Count) stateOrder[currentState].Cleanup();
    }
}
