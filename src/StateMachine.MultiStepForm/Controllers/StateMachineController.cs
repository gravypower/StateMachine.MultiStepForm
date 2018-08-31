using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using StateMachine.MultiStepForm.StateMachines;

namespace StateMachine.MultiStepForm.Controllers
{
    public abstract class StateMachineController<TState, TTrigger> : Controller
    {
        private const string StateKey = "State";

        protected AbstractStateMachine<TState, TTrigger> StateMachine { get; }

        protected TState State
        {
            get
            {
                var state = StateMachine.DefaultInitialState;
                var tempState = TempData[StateKey];

                if (tempState != null)
                {
                    state = (TState)tempState;
                }

                return state;
            }
            set => TempData[StateKey] = value;
        }

        public IEnumerable<string> Triggers => StateMachine.Triggers;

        protected StateMachineController(AbstractStateMachine<TState, TTrigger> stateMachine)
        {
            StateMachine = stateMachine;
        }

        public IActionResult Index()
        {
            ViewBag.Triggers = Triggers;
            ViewBag.State = StateMachine.CurrentState;
            return View(StateMachine.CurrentState.ToString());
        }

        [HttpPost]
        public IActionResult Index(string trigger)
        {
            return FireTrigger(trigger);
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

        private IActionResult TriggerFired()
        {
            State = StateMachine.CurrentState;
            return RedirectToAction("Index", ControllerContext.RouteData.Values["controller"].ToString());
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Method == "POST")
            {
                if (context.HttpContext.Request.HasFormContentType)
                {
                    var from = context.HttpContext.Request.Form;
                    if (from.ContainsKey(StateKey))
                    {
                        var state = StateMachine.ParseState(from[StateKey]);
                        StateMachine.ConfigureStateMachine(state);
                        State = state;
                    }
                }
            }
            else
            {
                StateMachine.ConfigureStateMachine(State);
            }
            
            base.OnActionExecuting(context);
        }
    }
}