using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using StateMachine.MultiStepForm.MagicStrings;
using StateMachine.MultiStepForm.Models;
using StateMachine.MultiStepForm.StateMachines;

namespace StateMachine.MultiStepForm.Controllers
{
    [AutoValidateAntiforgeryToken]
    public abstract class StateMachineController<TState, TTrigger> : Controller
    {
        private readonly IStateMachineMagicStrings<TTrigger> _stateMachineMagicStrings;
        private const string StateKey = "State";
        private const string TriggerKey = "Trigger";

        protected AbstractStateMachine<TState, TTrigger> StateMachine { get; }
        protected TState State
        {
            get => GetState();
            set => SetState(value);
        }

        protected StateMachineController(
            AbstractStateMachine<TState, TTrigger> stateMachine,
            IStateMachineMagicStrings<TTrigger> stateMachineMagicStrings)
        {
            _stateMachineMagicStrings = stateMachineMagicStrings;
            StateMachine = stateMachine;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Triggers = GetTriggerButtons();

            var stateToken = TokenGenerator.GetUniqueToken(64);
            TempData[$"{StateKey}-{stateToken}"] = StateMachine.CurrentState;
            ViewBag.State = stateToken;

            var state = StateMachine.CurrentState.ToString();
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

            if (from.ContainsKey(StateKey))
            {
                return (TState)TempData[$"{StateKey}-{from[StateKey]}"];
            }

            return StateMachine.DefaultInitialState;
        }

        protected virtual TState GetState()
        {
            if (IsPost())
            {
                return GetStateFromHiddenField();
            }

            var tempDataState = TempData[StateKey];
            if (tempDataState != null)
            {
                return (TState)tempDataState;
            }

            return StateMachine.DefaultInitialState;
        }

        protected virtual void SetState(TState state)
        {
            TempData[StateKey] = state;
        }

        private bool IsPost() => HttpContext.Request.Method == "POST";

        private IActionResult TriggerFired()
        {
            State = StateMachine.CurrentState;
            return Index(); //RedirectToAction("Index", ControllerContext.RouteData.Values["controller"].ToString());
        }

        private TTrigger GetTriggerFromToken(string triggerToken)
        {
            return (TTrigger)TempData[$"{TriggerKey}-{triggerToken}"];
        }

        private IEnumerable<TriggerButton> GetTriggerButtons()
        {
            foreach (var trigger in StateMachine.PermittedTriggers)
            {
                var triggerToken = TokenGenerator.GetUniqueToken(64);
                TempData[$"{TriggerKey}-{triggerToken}"] = trigger;

                var triggerLabel = trigger.ToString();

                if (_stateMachineMagicStrings.TriggerDescriptions.ContainsKey(trigger))
                    triggerLabel = _stateMachineMagicStrings.TriggerDescriptions[trigger];

                yield return new TriggerButton
                {
                    TriggerToken = triggerToken,
                    TriggerDescription = triggerLabel
                };
            }
        }
    }
}