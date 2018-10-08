using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using StateMachine.MultiStepForm.Contexts;
using StateMachine.MultiStepForm.Example.Models.DeepThought;

namespace StateMachine.MultiStepForm.Example.Controllers
{
    public class DeepThoughtController : StateMachineController<State, Trigger>
    {
        public DeepThoughtController(
            StateContext stateContext,
            IEnumerable<State> states,
            IEnumerable<Trigger> triggers,
            AbstractStateMachine<State, Trigger> stateMachine
            ) : base(stateContext, states, triggers, stateMachine)
        {
        }

        [HttpPost]
        [RequireRequestModelType(typeof(AnswerViewModel))]
        public IActionResult Index(string triggerToken, AnswerViewModel answerViewModel)
        {
            return FireTrigger(triggerToken, answerViewModel);
        }

        [HttpPost]
        [RequireRequestModelType(typeof(QuestionViewModel))]
        public IActionResult Index(string triggerToken, QuestionViewModel questionViewModel)
        {
            return FireTrigger(triggerToken, questionViewModel);
        }
    }
}