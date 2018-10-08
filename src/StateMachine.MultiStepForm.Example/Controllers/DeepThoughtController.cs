using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using StateMachine.MultiStepForm.Contexts;
using StateMachine.MultiStepForm.Example.Models.DeepThought;
using StateMachine.MultiStepForm.Example.StateMachines.DeepThought;

namespace StateMachine.MultiStepForm.Example.Controllers
{
    public class DeepThoughtController : StateMachineController<DeepThoughtStates.State, DeepThoughtTriggers.Trigger>
    {
        public DeepThoughtController(
            StateContext stateContext,
            TriggerContext triggerContext,
            IEnumerable<State> states,
            IEnumerable<Trigger> triggers,
            AbstractStateMachine<DeepThoughtStates.State, DeepThoughtTriggers.Trigger> stateMachine
            ) : base(stateContext, triggerContext, states, triggers, stateMachine)
        {
        }

        [HttpPost]
        [ActionMethodSelectorByModelType(typeof(AnswerViewModel))]
        public IActionResult Index(string triggerToken, AnswerViewModel answerViewModel)
        {
            return FireTrigger(triggerToken, answerViewModel);
        }

        [HttpPost]
        [ActionMethodSelectorByModelType(typeof(QuestionViewModel))]
        public IActionResult Index(string triggerToken, QuestionViewModel questionViewModel)
        {
            return FireTrigger(triggerToken, questionViewModel);
        }
    }
}