using System;
using System.Collections.Generic;
using System.Linq;
using Stateless;
using StateMachine.MultiStepForm.Contexts;

namespace StateMachine.MultiStepForm
{
    public abstract class AbstractStateMachine<TState, TTrigger>
        where TTrigger : Trigger
        where TState : State
    {
        protected readonly TriggerContext TriggerContext;
        protected readonly StateContext StateContext;
        private bool _isConfigured;

        protected StateMachine<TState, TTrigger> StateMachine { get; set; }
        
        private readonly IList<StateMachine<TState, TTrigger>.TriggerWithParameters> _triggersWithParameters;

        public IEnumerable<TTrigger> PermittedTriggers => StateMachine.PermittedTriggers;
        public TState CurrentState => StateMachine.State;
        public abstract TState DefaultInitialState { get; }

        protected AbstractStateMachine(
            TriggerContext triggerContext,
            StateContext stateContext)
        {
            TriggerContext = triggerContext;
            StateContext = stateContext;
            _triggersWithParameters = new List<StateMachine<TState, TTrigger>.TriggerWithParameters>();
        }

        public void ConfigureStateMachine(TState initialState)
        {
            if (_isConfigured) throw new Exception("StateMachine is already Configured");

            StateMachine = new StateMachine<TState, TTrigger>(initialState);
            DoConfigureStateMachine();
            StateMachine.Activate();
            _isConfigured = true;
        }

        public void Fire(TTrigger trigger)
        {
            StateMachine.Fire(trigger);
        }

        public void Fire<TArg>(TTrigger trigger, TArg arg)
        {
            TriggerContext.AddArg(trigger, arg);

            var twp = (StateMachine<TState, TTrigger>.TriggerWithParameters<TArg>) 
                _triggersWithParameters.SingleOrDefault(x => x.Trigger.Equals(trigger));
            
            if (twp != null)
            {
                twp.ValidateParameters(new object[] { arg });
                StateMachine.Fire(twp, arg);
                return;
            }

            StateMachine.Fire(trigger);
        }

        

        protected StateMachine<TState, TTrigger>.TriggerWithParameters<TModel> SetTriggerParameters<TModel>(TTrigger trigger)
        {
            var triggerWithParameter = StateMachine.SetTriggerParameters<TModel>(trigger);
            _triggersWithParameters.Add(triggerWithParameter);
            return triggerWithParameter;
        }

       

        protected abstract void DoConfigureStateMachine();
    }
}