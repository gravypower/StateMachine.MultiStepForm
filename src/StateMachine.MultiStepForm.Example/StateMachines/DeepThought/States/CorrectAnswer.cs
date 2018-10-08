using StateMachine.MultiStepForm.Contexts;
using StateMachine.MultiStepForm.Example.Models.DeepThought;
using StateMachine.MultiStepForm.Example.Specifications;
using StateMachine.MultiStepForm.Example.StateMachines.DeepThought.Triggers;

namespace StateMachine.MultiStepForm.Example.StateMachines.DeepThought.States
{
    public class CorrectAnswer : State
    {
        private readonly DeepThoughtTrigger _triggers;
        private readonly TriggerContext _triggerContext;
        private readonly Specification<AnswerViewModel> _meaningOfLifeSpecification;

        public CorrectAnswer(
            DeepThoughtTrigger triggers,
            TriggerContext triggerContext,
            Specification<AnswerViewModel> meaningOfLifeSpecification)
        {
            _triggers = triggers;
            _triggerContext = triggerContext;
            _meaningOfLifeSpecification = meaningOfLifeSpecification;
        }

        public bool IsCorrectAnswer()
        {
            var arg = _triggerContext.GetArg<AnswerViewModel>(_triggers.AskDeepThought);
            return _meaningOfLifeSpecification.IsSatisfiedBy(arg);
        }

        public bool IsNotCorrectAnswer()
        {
            return !IsCorrectAnswer();
        }
    }
}