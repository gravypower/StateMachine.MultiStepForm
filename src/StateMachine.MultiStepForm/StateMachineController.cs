using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using StateMachine.MultiStepForm.Contexts;

namespace StateMachine.MultiStepForm
{
    [AutoValidateAntiforgeryToken]
    public abstract class StateMachineController<TState, TTrigger> : Controller
        where TTrigger : Trigger
        where TState : State
    {
        private const string StateInputKey = "stateToken";

        private readonly StateContext _stateContext;
        private readonly TriggerContext _triggerContext;
        private readonly IEnumerable<State> _states;
        private readonly IEnumerable<Trigger> _triggers;

        protected AbstractStateMachine<TState, TTrigger> StateMachine { get; }
        protected TState State => GetState();

        protected StateMachineController(
            StateContext stateContext,
            TriggerContext triggerContext,
            IEnumerable<State> states,
            IEnumerable<Trigger> triggers,
            AbstractStateMachine<TState, TTrigger> stateMachine)
        {
            _stateContext = stateContext;
            _triggerContext = triggerContext;
            _states = states;
            _triggers = triggers;
            StateMachine = stateMachine;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Triggers = GetTriggerButtons();

            var stateToken = _stateContext.GetStateToken(StateMachine.CurrentState);
            ViewBag.State = stateToken;

            var state = StateMachine.CurrentState.GetType().Name;
            var model = _stateContext.GetModel(StateMachine.CurrentState);
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

            if (!from.ContainsKey(StateInputKey)) return StateMachine.DefaultInitialState;

            var state = _stateContext.GetStateKeyFromToken(from[StateInputKey]);
            return _states.Single(s => s.Key == state) as TState;
        }

        protected TState GetState()
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
            var trigger = _triggerContext.GetTriggerKeyFromToken(triggerToken);
            return _triggers.Single(t => t.Key == trigger) as TTrigger;
        }

        private IEnumerable<TriggerButton> GetTriggerButtons() =>
            StateMachine.PermittedTriggers.Select(trigger =>
                new TriggerButton
                {
                    TriggerToken = _triggerContext.GetTriggerToken(trigger),
                    Trigger = trigger
                });
    }
}