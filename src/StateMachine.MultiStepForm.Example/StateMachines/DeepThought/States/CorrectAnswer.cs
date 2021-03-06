﻿using StateMachine.MultiStepForm.Contexts;
using StateMachine.MultiStepForm.Example.Models.DeepThought;
using StateMachine.MultiStepForm.Example.Specifications;

namespace StateMachine.MultiStepForm.Example.StateMachines.DeepThought.States
{
    public class CorrectAnswer : DeepThoughtStates.State
    {
        private readonly TriggerContext _triggerContext;
        private readonly Specification<AnswerViewModel> _meaningOfLifeSpecification;

        public CorrectAnswer(
            TriggerContext triggerContext,
            Specification<AnswerViewModel> meaningOfLifeSpecification)
        {
            _triggerContext = triggerContext;
            _meaningOfLifeSpecification = meaningOfLifeSpecification;
        }

        public bool IsCorrectAnswer()
        {
            var arg = _triggerContext.GetArg<AnswerViewModel>(DeepThoughtTriggers.AskDeepThought);
            return _meaningOfLifeSpecification.IsSatisfiedBy(arg);
        }

        public bool IsNotCorrectAnswer()
        {
            return !IsCorrectAnswer();
        }
    }
}