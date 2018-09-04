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
            get => GetState();
            set => SetState(value);
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
            StateMachine.ConfigureStateMachine(State);
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

        protected TState GetStateFromHiddenField()
        {
            if (!HttpContext.Request.HasFormContentType)
                return StateMachine.DefaultInitialState;

            var from = HttpContext.Request.Form;

            if (from.ContainsKey(StateKey))
                return StateMachine.ParseState(from[StateKey]);

            return StateMachine.DefaultInitialState;
        }

        protected virtual TState GetState()
        {
            if (HttpContext.Request.Method == "POST")
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
    }
}