
using System;
using JetBrains.Annotations;
using UnityEngine;

public interface IState<TStatus>
{
    public void Setup();
    public ITransition Update();
    public void LateUpdate();
    public void FixedUpdate();
    public void Cleanup();
}

/**
 * The base component of a state machine. Each individual action is its own state
 *
 * Note that while it is helpful to think of States as "only being run once", if they are part of a state machine
 * which loops, they may actually be run more than once. Keep this in mind when writing Setup and Cleanup.
 *
 * Properties:
 * - gameStatus: contains all the information about the current game status, both to query and modify
 * - stateStartTime: at what Time.time this state started running, or -1 if it is not currently running
 * - timeSinceStart: how long this state has been running for, or -1 if it is not currently running
 * - name: what this state is doing (for debugging purposes)
 *
 * Methods:
 * - Setup: run once when this state starts
 * - Update: runs every loop this state is active, during Unity's Update. Returns null if the state should continue
 *      running, or some Transition type if not
 * - Run: where subclasses define what happens in Update. Runs every loop this state is active, during Unity's Update.
 *      Returns null if the state should continue running, or some Transition type if not
 * - LateUpdate: runs every loop this state is active, during Unity's LateUpdate. Note that transitions happen in
 *      Update, so it is possible that LateUpdate will be called the loop before the first Update is called
 * - FixedUpdate: runs whenever Unity's FixedUpdate is called when this state is active
 * - Cleanup: runs once when this state finishes
 *
 * Transitions:
 *      Transitions define when this state should finish and with what status. A transition value of null means that
 *      the state has not finished running. Any other value means the state is done, and it may or may not contain
 *      extra information. Most states use a DefaultTransition, which has only one non-null transition value, saying
 *      only that the state is done. However, states may convey more information, such as a Success/Failure condition
 *      using SuccessTransition - this will return `Success` when the state "succeeds", and `Fail` when the state
 *      `Fails`. See Transition for how to define transitions
 */
public class State<TStatus, TTransition> : IState<TStatus> where TTransition : ITransition
{
    protected TStatus status;
    protected float stateStartTime;
    protected string name;
    protected TTransition transition;

    protected float timeSinceStart => stateStartTime == -1 ? -1 : Time.time - stateStartTime;

    public State(TStatus status, string id = null)
    {
        this.status = status;
        name = id;
        stateStartTime = -1;
        transition = default;
    }
    
    public override string ToString()
    {
        return name ?? GetType().Name;
    }

    public virtual void Setup()
    {
        Debug.Log($"Starting to run {this}");
        stateStartTime = Time.time;
        transition = default;
    }

    public TTransition Update()
    {
        transition = Run();
        return transition;
    }

    protected virtual TTransition Run()
    {
        return default;
    }

    ITransition IState<TStatus>.Update() => Update();
    
    public virtual void LateUpdate() {}
    
    public virtual void FixedUpdate() {}

    public virtual void Cleanup()
    {
        stateStartTime = -1;
        Debug.Log($"Finished running {this}");
    }
}
