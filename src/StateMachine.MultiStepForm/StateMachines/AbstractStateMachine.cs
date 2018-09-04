using System;
using System.Collections.Generic;
using System.Linq;
using Stateless;
using Stateless.Graph;

namespace StateMachine.MultiStepForm.StateMachines
{
    public abstract class AbstractStateMachine<TState, TTrigger>
    {
        private bool _isConfigured;

        protected StateMachine<TState, TTrigger> StateMachine { get; set; }

        private readonly IDictionary<TTrigger, object> _triggerArgs;
        private readonly IDictionary<TState, object> _stateModels;
        private readonly IList<StateMachine<TState, TTrigger>.TriggerWithParameters> _triggersWithParameters;

        public IEnumerable<string> PermittedTriggers => StateMachine.PermittedTriggers.Select(t => t.ToString());
        public TState CurrentState => StateMachine.State;
        public abstract TState DefaultInitialState { get; }

        protected AbstractStateMachine()
        {
            _triggersWithParameters = new List<StateMachine<TState, TTrigger>.TriggerWithParameters>();
            _triggerArgs = new Dictionary<TTrigger, object>();
            _stateModels = new Dictionary<TState, object>();

            if (!typeof(TState).IsEnum)
            {
                throw new ArgumentException("TState must be an enumerated type");
            }

            if (!typeof(TTrigger).IsEnum)
            {
                throw new ArgumentException("TTrigger must be an enumerated type");
            }
        }

        public void Activate()
        {
            StateMachine.Activate();
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

            _triggerArgs.Add(t, arg);

            var twp = (StateMachine<TState, TTrigger>.TriggerWithParameters<TArg>) 
                _triggersWithParameters.SingleOrDefault(x => x.Trigger.Equals(t));
            
            if (twp != null)
            {
                twp.ValidateParameters(new object[] { arg });
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

        public void SetModel(TState state, object model)
        {
            _stateModels.Add(state, model);
        }

        public object GetModel(TState state)
        {
            return _stateModels.ContainsKey(state) ? _stateModels[state] : null;
        }

        public string DotGraph()
        {
            return UmlDotGraph.Format(StateMachine.GetInfo());
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
    }
}