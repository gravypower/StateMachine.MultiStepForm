using Microsoft.AspNetCore.Mvc;
using StateMachine.MultiStepForm.MagicStrings;
using StateMachine.MultiStepForm.Models.DeepThought;
using StateMachine.MultiStepForm.StateMachines;
using StateMachine.MultiStepForm.StateMachines.DeepThought;

namespace StateMachine.MultiStepForm.Controllers
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