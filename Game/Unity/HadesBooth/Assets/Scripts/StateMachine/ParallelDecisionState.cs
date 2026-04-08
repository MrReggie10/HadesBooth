
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * A state which can run multiple states at the same time, transitioning via one of the sub-State transitions
 *
 * Update and LateUpdate are called for each given sub-State each loop until that State says it is done running. This
 * State will continue running until all sub-States are done, at which point it will return the transition value
 *
 * Methods:
 * - AddState: Add a state to be run in parallel
 * - SetTransitionState: Set the sub-State which defines the Transition value for this State. MUST be called once before
 *      this State runs. Will call AddState as well, so no need to call both for this sub-State.
 *
 * By: Ben Morris
 */
public class ParallelDecisionState<TStatus, TTransition> : State<TStatus, TTransition> where TTransition : ITransition
{
    protected ISet<IState<TStatus>> states;
    protected ISet<IState<TStatus>> runningStates;
    protected State<TStatus, TTransition> transitionState;
    protected TTransition finalTransition;
    
    public ParallelDecisionState(TStatus status, string id = null) : base(status, id)
    {
        states = new HashSet<IState<TStatus>>();
        runningStates = new HashSet<IState<TStatus>>();
    }

    public void AddState(IState<TStatus> state)
    {
        states.Add(state);
    }

    public void SetTransitionState(State<TStatus, TTransition> transitionState)
    {
        if (!states.Contains(transitionState))
        {
            states.Add(transitionState);
        }
        this.transitionState = transitionState;
    }

    public override void Setup()
    {
        base.Setup();
        if (transitionState == null) Debug.LogError("Started ParallelDecisionState without transition state defined");
        foreach (IState<TStatus> state in states)
        {
            state.Setup();
            runningStates.Add(state);
        }

        finalTransition = default;
    }

    protected override TTransition Run()
    {
        TTransition baseTrans = base.Run();
        if (baseTrans != null) return baseTrans;

        ISet<IState<TStatus>> toRemove = new HashSet<IState<TStatus>>();
        foreach (IState<TStatus> state in runningStates)
        {
            ITransition transition = state.Update();
            if (transition != null)
            {
                toRemove.Add(state);
                if (state is State<TStatus, TTransition> tState && tState == transitionState && transition is TTransition tTransition) // last part must be true if first two are, just need conversion
                {
                    finalTransition = tTransition;
                }
            }
        }

        foreach (IState<TStatus> state in toRemove)
        {
            runningStates.Remove(state);
        }

        return runningStates.Any() ? default : finalTransition;
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        foreach (IState<TStatus> state in runningStates)
        {
            state.LateUpdate();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        foreach (IState<TStatus> state in runningStates)
        {
            state.FixedUpdate();
        }
    }

    public override void Cleanup()
    {
        base.Cleanup();
        runningStates.Clear();
    }
}
