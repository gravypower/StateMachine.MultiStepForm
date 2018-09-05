using Microsoft.AspNetCore.Mvc;
using StateMachine.MultiStepForm.Example.MagicStrings;
using StateMachine.MultiStepForm.Example.Models.DeepThought;
using StateMachine.MultiStepForm.Example.StateMachines.DeepThought;

namespace StateMachine.MultiStepForm.Example.Controllers
{
    public class DeepThoughtController : StateMachineController<State, Trigger>
    {
        public DeepThoughtController(
            AbstractStateMachine<State, Trigger> stateMachine,
            IDeepThoughtMagicStrings deepThoughtMagicStrings
            ) : base(stateMachine, deepThoughtMagicStrings)
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