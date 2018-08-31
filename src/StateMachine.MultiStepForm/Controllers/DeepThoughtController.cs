using Microsoft.AspNetCore.Mvc;
using StateMachine.MultiStepForm.Models.DeepThought;
using StateMachine.MultiStepForm.StateMachines;
using StateMachine.MultiStepForm.StateMachines.DeepThought;

namespace StateMachine.MultiStepForm.Controllers
{
    public class DeepThoughtController : StateMachineController<State, Trigger>
    {
        public DeepThoughtController(AbstractStateMachine<State, Trigger> stateMachine) : base(stateMachine)
        {
        }

        [HttpPost]
        public IActionResult MeaningOfLife(string trigger, AnswerViewModel model)
        {
            return FireTrigger(trigger, model);
        }

        [HttpPost]
        public IActionResult QuestionToTheAnswer(string trigger, QuestionViewModel model)
        {
            return FireTrigger(trigger, model);
        }
    }
}