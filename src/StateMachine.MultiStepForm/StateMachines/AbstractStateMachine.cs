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

        private readonly IDictionary<TTrigger, object> _triggerArgs;
        private readonly IDictionary<TState, object> _stateModels;
        private readonly IList<StateMachine<TState, TTrigger>.TriggerWithParameters> _triggersWithParameters;

        public IEnumerable<TTrigger> PermittedTriggers => StateMachine.PermittedTriggers;
        public TState CurrentState => StateMachine.State;
        public abstract TState DefaultInitialState { get; }

        protected AbstractStateMachine()
        {
            _triggersWithParameters = new List<StateMachine<TState, TTrigger>.TriggerWithParameters>();
            _triggerArgs = new Dictionary<TTrigger, object>();
            _stateModels = new Dictionary<TState, object>();

            GuardTState();
            GuardTTrigger();
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
            _triggerArgs.Add(trigger, arg);

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

        public void SetModel(TState state, object model)
        {
            _stateModels.Add(state, model);
        }

        public object GetModel(TState state)
        {
            return _stateModels.ContainsKey(state) ? _stateModels[state] : null;
        }

        protected StateMachine<TState, TTrigger>.TriggerWithParameters<TModel> SetTriggerParameters<TModel>(TTrigger trigger)
        {
            var triggerWithParameter = StateMachine.SetTriggerParameters<TModel>(trigger);
            _triggersWithParameters.Add(triggerWithParameter);
            return triggerWithParameter;
        }

        protected TArg GetArg<TArg>(TTrigger trigger)
        {
            if (!_triggerArgs.ContainsKey(trigger))
                return default(TArg);

            if (!(_triggerArgs[trigger] is TArg))
                return default(TArg);

            return (TArg)_triggerArgs[trigger];
        }

        protected abstract void DoConfigureStateMachine();

        private static void GuardTTrigger()
        {
            if (!typeof(TTrigger).IsEnum)
            {
                throw new ArgumentException("TTrigger must be an enumerated type");
            }
        }

        private static void GuardTState()
        {
            if (!typeof(TState).IsEnum)
            {
                throw new ArgumentException("TState must be an enumerated type");
            }
        }
    }
}