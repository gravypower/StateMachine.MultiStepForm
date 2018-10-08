using System.Collections.Generic;
using System.Linq;

namespace StateMachine.MultiStepForm
{
    public abstract class AbstractStateMachineStates
    {
        protected readonly IEnumerable<State> States;

        protected AbstractStateMachineStates(IEnumerable<State> states)
        {
            States = states;
        }

        protected TState GetState<TState>()
            where TState : State
        {
            return States.Single(s => s.GetType() == typeof(TState)) as TState;
        }
    }
}