using StateMachine.MultiStepForm.Contexts;
using StateMachine.MultiStepForm.Example.Models.DeepThought;
using StateMachine.MultiStepForm.Example.StateMachines.DeepThought.States;
using StateMachine.MultiStepForm.Example.StateMachines.DeepThought.Triggers;

namespace StateMachine.MultiStepForm.Example.StateMachines.DeepThought
{
    public class DeepThoughtStateMachine : AbstractStateMachine<State, Trigger>
    {
        private readonly DeepThoughtStates _states;
        private readonly DeepThoughtTrigger _triggers;

        public override State DefaultInitialState => _states.MeaningOfLife;

        public DeepThoughtStateMachine(
            StateContext stateContext,
            TriggerContext triggerContext,
            DeepThoughtStates states,
            DeepThoughtTrigger triggers) : base(triggerContext, stateContext)
        {
            _states = states;
            _triggers = triggers;
        }

        protected override void DoConfigureStateMachine()
        {
            StateMachine.Configure(_states.MeaningOfLife)
                .PermitIf(_triggers.AskDeepThought, _states.CorrectAnswer, _states.CorrectAnswer.IsCorrectAnswer)
                .PermitIf(_triggers.AskDeepThought, _states.IncorrectAnswer, _states.CorrectAnswer.IsNotCorrectAnswer);

            StateMachine.Configure(_states.CorrectAnswer)
                .Permit(_triggers.WhatIsTheQuestion, _states.QuestionToTheAnswer);

            StateMachine.Configure(_states.IncorrectAnswer)
                .Permit(_triggers.TryAgain, _states.MeaningOfLife)
                .Permit(_triggers.WhatIsTheQuestion, _states.QuestionToTheAnswer);

            StateMachine.Configure(_states.QuestionToTheAnswer)
                .Permit(_triggers.YourQuestionToTheAnswer, _states.SoLongAndThanksForAllTheFish);

            var yourQuestionToTheAnswerTrigger = SetTriggerParameters<QuestionViewModel>(_triggers.YourQuestionToTheAnswer);
            
            StateMachine.Configure(_states.SoLongAndThanksForAllTheFish)
                .OnEntryFrom(yourQuestionToTheAnswerTrigger, _triggers.YourQuestionToTheAnswer.YourAnswer)
                .OnActivate(_states.SoLongAndThanksForAllTheFish.GetQuestion);
        }       
    }
}
