using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;

namespace StateMachine.MultiStepForm
{
    [AutoValidateAntiforgeryToken]
    public abstract class StateMachineController<TState, TTrigger> : Controller
        where TTrigger : Trigger
        where TState : State
    {
        private readonly IEnumerable<State> _states;
        private readonly IEnumerable<Trigger> _triggers;
        private const string StateKey = "stateToken";
        private const string TriggerKey = "Trigger";

        protected AbstractStateMachine<TState, TTrigger> StateMachine { get; }
        protected TState State => GetState();

        protected StateMachineController(
            IEnumerable<State> states,
            IEnumerable<Trigger> triggers,
            AbstractStateMachine<TState, TTrigger> stateMachine)
        {
            _states = states;
            _triggers = triggers;
            StateMachine = stateMachine;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Triggers = GetTriggerButtons();

            var stateToken = TokenGenerator.GetUniqueToken(64);
            TempData[$"{StateKey}-{stateToken}"] = StateMachine.CurrentState.GetType().AssemblyQualifiedName;
            ViewBag.State = stateToken;

            var state = StateMachine.CurrentState.GetType().Name;
            var model = StateMachine.GetModel(StateMachine.CurrentState);
            return model == null ? View(state) : View(state, model);
        }

        [HttpPost]
        public IActionResult Index(string triggerToken)
        {
            var trigger = GetTriggerFromToken(triggerToken);
            StateMachine.Fire(trigger);
            return TriggerFired();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var t = _states.Single(s => s.GetType().AssemblyQualifiedName == State.GetType().AssemblyQualifiedName);

            StateMachine.ConfigureStateMachine(State);
            base.OnActionExecuting(context);
        }

        protected IActionResult FireTrigger<TArg>(string triggerToken, TArg arg)
        {
            var trigger = GetTriggerFromToken(triggerToken);
            StateMachine.Fire(trigger, arg);
            return TriggerFired();
        }

        protected TState GetStateFromHiddenField()
        {
            if (!HttpContext.Request.HasFormContentType)
                return StateMachine.DefaultInitialState;

            var from = HttpContext.Request.Form;

            if (!from.ContainsKey(StateKey)) return StateMachine.DefaultInitialState;

            var state = (string)TempData[$"{StateKey}-{from[StateKey]}"];
            return _states.Single(s => s.GetType().AssemblyQualifiedName == state) as TState;
        }

        protected virtual TState GetState()
        {
            return IsPost() ? GetStateFromHiddenField() : StateMachine.DefaultInitialState;
        }

        private bool IsPost() => HttpContext.Request.Method == "POST";

        private IActionResult TriggerFired()
        {
            return Index();
        }

        private TTrigger GetTriggerFromToken(string triggerToken)
        {
            var trigger = (string)TempData[$"{TriggerKey}-{triggerToken}"];
            return _triggers.Single(t => t.GetType().AssemblyQualifiedName == trigger) as TTrigger;
        }

        private IEnumerable<TriggerButton> GetTriggerButtons()
        {
            foreach (var trigger in StateMachine.PermittedTriggers)
            {
                var triggerToken = TokenGenerator.GetUniqueToken(64);
                TempData[$"{TriggerKey}-{triggerToken}"] = trigger.GetType().AssemblyQualifiedName;


                yield return new TriggerButton
                {
                    TriggerToken = triggerToken,
                    Trigger = trigger
                };
            }
        }
    }
}