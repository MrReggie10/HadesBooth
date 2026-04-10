
using System.Collections.Generic;
using System.Linq;

/**
 * A state which can run multiple states at the same time
 *
 * Update and LateUpdate are called for each given sub-State each loop until that State says it is done running. This
 * State will continue running until all sub-States are done, at which point it will return DefaultTransition.Default
 *
 * Methods:
 * - AddState: Add a State to be run in parallel
 *
 * By: Ben Morris
 */
public class ParallelState<TStatus> : State<TStatus, DefaultTransition>
{
    protected ISet<IState<TStatus>> states;
    protected ISet<IState<TStatus>> runningStates;
    
    public ParallelState(TStatus status, string id = null) : base(status, id)
    {
        states = new HashSet<IState<TStatus>>();
        runningStates = new HashSet<IState<TStatus>>();
    }

    public void AddState(IState<TStatus> state)
    {
        states.Add(state);
    }

    public override void Setup()
    {
        base.Setup();
        foreach (IState<TStatus> state in states)
        {
            state.Setup();
            runningStates.Add(state);
        }
    }

    protected override DefaultTransition Run()
    {
        DefaultTransition baseTrans = base.Run();
        if (baseTrans != null) return baseTrans;

        ISet<IState<TStatus>> toRemove = new HashSet<IState<TStatus>>();
        foreach (IState<TStatus> state in runningStates)
        {
            ITransition transition = state.Update();
            if (transition != null)
            {
                toRemove.Add(state);
                state.Cleanup();
            }
        }

        foreach (IState<TStatus> state in toRemove)
        {
            runningStates.Remove(state);
        }

        return runningStates.Any() ? null : DefaultTransition.Default;
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
