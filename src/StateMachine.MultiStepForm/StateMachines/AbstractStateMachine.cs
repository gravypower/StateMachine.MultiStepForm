using System;
using System.Collections.Generic;
using System.Linq;
using Stateless;

namespace StateMachine.MultiStepForm.StateMachines
{
    public abstract class AbstractStateMachine<TState, TTrigger>
    {
        private bool _isConfigured;

        protected StateMachine<TState, TTrigger> StateMachine { get; set; }
        protected IDictionary<TTrigger, object> TriggerArgs { get; } = new Dictionary<TTrigger, object>();

        public IEnumerable<string> Triggers => StateMachine.PermittedTriggers.Select(t => t.ToString());
        public IList<StateMachine<TState, TTrigger>.TriggerWithParameters> TriggersWithParameters { get; }
        public TState CurrentState => StateMachine.State;

        public abstract TState DefaultInitialState { get; }

        protected AbstractStateMachine()
        {
            TriggersWithParameters = new List<StateMachine<TState, TTrigger>.TriggerWithParameters>();
            if (!typeof(TState).IsEnum)
            {
                throw new ArgumentException("TState must be an enumerated type");
            }

            if (!typeof(TTrigger).IsEnum)
            {
                throw new ArgumentException("TTrigger must be an enumerated type");
            }
        }

        public void ConfigureStateMachine(TState initialState)
        {
            if (_isConfigured) throw new Exception("StateMachine is already Configured");

            StateMachine = new StateMachine<TState, TTrigger>(initialState);
            DoConfigureStateMachine();
            _isConfigured = true;
        }

        public void Fire(string trigger)
        {
            var t = ParseTrigger(trigger);
            StateMachine.Fire(t);
        }

        public void Fire<TArg>(string trigger, TArg arg)
        {
            var t = ParseTrigger(trigger);

            TriggerArgs.Add(t, arg);

            var twp = (StateMachine<TState, TTrigger>.TriggerWithParameters<TArg>) 
                TriggersWithParameters.SingleOrDefault(x => x.Trigger.Equals(t));

            if (twp != null)
            {
                StateMachine.Fire(twp, arg);
                return;
            }

            StateMachine.Fire(t);
        }

        public TTrigger ParseTrigger(string trigger)
        {
            return (TTrigger)Enum.Parse(typeof(TTrigger), trigger);
        }

        public TState ParseState(string state)
        {
            return (TState)Enum.Parse(typeof(TState), state);
        }

        protected TArg GetArg<TArg>(TTrigger trigger)
        {
            if (TriggerArgs.ContainsKey(trigger))
                return (TArg)TriggerArgs[trigger];

            return default(TArg);
        }

        protected abstract void DoConfigureStateMachine();
    }
}