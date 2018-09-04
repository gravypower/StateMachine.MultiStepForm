using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using StateMachine.MultiStepForm.Models;
using StateMachine.MultiStepForm.StateMachines;

namespace StateMachine.MultiStepForm.Controllers
{
    public abstract class StateMachineController<TState, TTrigger> : Controller
    {
        private const string StateKey = "State";

        protected AbstractStateMachine<TState, TTrigger> StateMachine { get; }

        protected TState State
        {
            get => GetStateFromTempDataOrDefault();
            set => TempData[StateKey] = value;
        }

        protected StateMachineController(AbstractStateMachine<TState, TTrigger> stateMachine)
        {
            StateMachine = stateMachine;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Triggers = GetTriggerButtons();
            ViewBag.State = StateMachine.CurrentState;
            var state = StateMachine.CurrentState.ToString();
            var model = StateMachine.GetModel(StateMachine.CurrentState);

            return model == null ? View(state) : View(state, model);
        }

        [HttpPost]
        public IActionResult Index(string trigger)
        {
            return FireTrigger(trigger);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Method == "POST")
            {
                SetStateFromHiddenField(context);
            }
            else
            {
                StateMachine.ConfigureStateMachine(State);
            }

            StateMachine.Activate();
            base.OnActionExecuting(context);
        }

        protected IActionResult FireTrigger(string trigger)
        {
            StateMachine.Fire(trigger);
            return TriggerFired();
        }

        protected IActionResult FireTrigger<TArg>(string trigger, TArg arg)
        {
            StateMachine.Fire(trigger, arg);
            return TriggerFired();
        }

        protected virtual IEnumerable<TriggerButton> GetTriggerButtons()
        {
            return StateMachine.PermittedTriggers.Select(trigger => 
                new TriggerButton
                {
                    Trigger = trigger,
                    TriggerDescription = trigger
                });
        }

        private IActionResult TriggerFired()
        {
            State = StateMachine.CurrentState;
            return RedirectToAction("Index", ControllerContext.RouteData.Values["controller"].ToString());
        }

        private void SetStateFromHiddenField(ActionContext context)
        {
            if (!context.HttpContext.Request.HasFormContentType) return;

            var from = context.HttpContext.Request.Form;

            if (!from.ContainsKey(StateKey)) return;

            var state = StateMachine.ParseState(from[StateKey]);
            StateMachine.ConfigureStateMachine(state);
            State = state;
        }

        private TState GetStateFromTempDataOrDefault()
        {
            var state = StateMachine.DefaultInitialState;
            var tempState = TempData[StateKey];

            if (tempState != null)
            {
                state = (TState)tempState;
            }

            return state;
        }
    }
}