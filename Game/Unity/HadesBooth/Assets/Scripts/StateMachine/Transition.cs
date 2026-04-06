
using System;


public interface ITransition
{
    public Enum GetTransition();
}

/**
 * A Transition is a wrapper on an Enum which defines how a State can transition
 * Think of it solely as an Enum which can be null.
 *
 * Some general-purpose Transition types are defined below.
 *
 * By: Ben Morris
 */
public class Transition<TTransition> : ITransition where TTransition : Enum
{
    public readonly TTransition transition;

    public Transition(TTransition transition)
    {
        this.transition = transition;
    }

    public Enum GetTransition() => transition;

    public override string ToString()
    {
        return transition.ToString();
    }

    public override bool Equals(object obj)
    {
        if (obj is Transition<TTransition> other)
        {
            return other.transition.Equals(transition);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return transition.GetHashCode();
    }
}

public enum DefaultTransitionEnum
{
    Default
}

public class DefaultTransition : Transition<DefaultTransitionEnum>
{
    public static DefaultTransition Default = new(DefaultTransitionEnum.Default);
    
    public DefaultTransition(DefaultTransitionEnum transition) : base(transition) {}
}

public enum SuccessEnum
{
    Success, Fail
}

public class SuccessTransition : Transition<SuccessEnum>
{
    public static SuccessTransition Success = new(SuccessEnum.Success);
    public static SuccessTransition Fail = new(SuccessEnum.Fail);
    
    public SuccessTransition(SuccessEnum transition) : base(transition) {}
}
