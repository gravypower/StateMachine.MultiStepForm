using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StateMachine.MultiStepForm.MagicStrings;
using StateMachine.MultiStepForm.Models;
using StateMachine.MultiStepForm.Models.DeepThought;
using StateMachine.MultiStepForm.StateMachines;
using StateMachine.MultiStepForm.StateMachines.DeepThought;

namespace StateMachine.MultiStepForm.Controllers
{
    public class DeepThoughtController : StateMachineController<State, Trigger>
    {
        private readonly IDeepThoughtMagicStrings _deepThoughtMagicStrings;

        public DeepThoughtController(
            AbstractStateMachine<State, Trigger> stateMachine,
            IDeepThoughtMagicStrings deepThoughtMagicStrings
            ) : base(stateMachine)
        {
            _deepThoughtMagicStrings = deepThoughtMagicStrings;
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

        protected override IEnumerable<TriggerButton> GetTriggerButtons()
        {
            return StateMachine.Triggers.Select(trigger =>
                new TriggerButton
                {
                    Trigger = trigger,
                    TriggerDescription = _deepThoughtMagicStrings.TriggerDescriptions[trigger]
                });
        }
    }
}