
using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * A state machine that can model any Markovian process (handles branches and loops)
 *
 * Methods:
 * - SetInitialState: define the state at which this NetworkedStateMachine starts
 * - AddTransition: define a transition from one state to the next. If the start state for this transition has multiple
 *      transition options (i.e. is not DefaultTransition), you must define which transition value will cause the state
 *      machine to move to this next state
 * - AddExitTransition: define a transition that will cause this state machine to finish. Must define both a state and
 *      which transition out of that state will cause the state machine to exit.
 *
 * By: Ben Morris
 */
public class NetworkedStateMachine<TStatus> : State<TStatus, DefaultTransition>
{
    protected Dictionary<IState<TStatus>, Dictionary<ITransition, IState<TStatus>>> transitions;
    protected Dictionary<IState<TStatus>, ISet<ITransition>> exitTransitions;
    protected IState<TStatus> initialState;

    protected IState<TStatus> currentState;

    public NetworkedStateMachine(TStatus status, string id = null) : base(status, id)
    {
        transitions = new Dictionary<IState<TStatus>, Dictionary<ITransition, IState<TStatus>>>();
        exitTransitions = new Dictionary<IState<TStatus>, ISet<ITransition>>();
    }

    public void SetInitialState(IState<TStatus> state)
    {
        initialState = state;
    }

    public void AddTransition<TTransition>(State<TStatus, TTransition> startState, TTransition transition,
        IState<TStatus> targetState) where TTransition : ITransition
    {
        if (!transitions.ContainsKey(startState))
        {
            transitions.Add(startState, new Dictionary<ITransition, IState<TStatus>>());
        }
        transitions[startState].Add(transition, targetState);
    }

    public void AddTransition(State<TStatus, DefaultTransition> startState, IState<TStatus> targetState)
    {
        AddTransition(startState, DefaultTransition.Default, targetState);
    }

    public void AddExitTransition<TTransition>(State<TStatus, TTransition> state, TTransition exitTransition)
        where TTransition : ITransition
    {
        if (!exitTransitions.ContainsKey(state))
        {
            exitTransitions.Add(state, new HashSet<ITransition>());
        }

        exitTransitions[state].Add(exitTransition);
    }

    public void AddExitTransition(State<TStatus, DefaultTransition> state)
    {
        AddExitTransition(state, DefaultTransition.Default);
    }

    public override void Setup()
    {
        base.Setup();
        currentState = initialState;
        currentState.Setup();
    }

    protected override DefaultTransition Run()
    {
        DefaultTransition baseUpdate = base.Run();
        if (baseUpdate != null) return baseUpdate;

        if (currentState == null) return DefaultTransition.Default;

        ITransition transition = currentState.Update();
        if (transition == null) return null;
        
        currentState.Cleanup();
        if (IsExitTransition(currentState, transition))
        {
            Debug.Log($"Exiting from {currentState} via {transition.GetTransition()}");
            return DefaultTransition.Default;
        }

        IState<TStatus> nextState = GetNextState(currentState, transition);
        Debug.Log($"Transitioning from {currentState} via {transition.GetTransition()} to {nextState}");
        nextState.Setup();
        currentState = nextState;
        return null;
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        currentState?.LateUpdate();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        currentState?.FixedUpdate();
    }

    public override void Cleanup()
    {
        base.Cleanup();
        currentState = null;
    }

    protected bool IsExitTransition(IState<TStatus> state, ITransition transition)
    {
        return exitTransitions.TryGetValue(state, out var exits) && exits.Contains(transition);
    }

    protected IState<TStatus> GetNextState(IState<TStatus> startState, ITransition transition)
    {
        return transitions.GetValueOrDefault(startState)?[transition];
    }
}
