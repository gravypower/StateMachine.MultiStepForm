using StateMachine.MultiStepForm.Contexts;
using StateMachine.MultiStepForm.Example.Models.DeepThought;

namespace StateMachine.MultiStepForm.Example.StateMachines.DeepThought
{
    public class DeepThoughtStateMachine : AbstractStateMachine<State, Trigger>
    {
        public override State DefaultInitialState => DeepThoughtStates.MeaningOfLife;

        public DeepThoughtStateMachine(
            StateContext stateContext,
            TriggerContext triggerContext) : base(triggerContext, stateContext)
        {
        }

        protected override void DoConfigureStateMachine()
        {
            StateMachine.Configure(DeepThoughtStates.MeaningOfLife)
                .PermitIf(DeepThoughtTriggers.AskDeepThought, DeepThoughtStates.CorrectAnswer, DeepThoughtStates.CorrectAnswer.IsCorrectAnswer)
                .PermitIf(DeepThoughtTriggers.AskDeepThought, DeepThoughtStates.IncorrectAnswer, DeepThoughtStates.CorrectAnswer.IsNotCorrectAnswer);

            StateMachine.Configure(DeepThoughtStates.CorrectAnswer)
                .Permit(DeepThoughtTriggers.WhatIsTheQuestion, DeepThoughtStates.QuestionToTheAnswer);

            StateMachine.Configure(DeepThoughtStates.IncorrectAnswer)
                .Permit(DeepThoughtTriggers.TryAgain, DeepThoughtStates.MeaningOfLife)
                .Permit(DeepThoughtTriggers.WhatIsTheQuestion, DeepThoughtStates.QuestionToTheAnswer);

            StateMachine.Configure(DeepThoughtStates.QuestionToTheAnswer)
                .Permit(DeepThoughtTriggers.YourQuestionToTheAnswer, DeepThoughtStates.SoLongAndThanksForAllTheFish);

            var yourQuestionToTheAnswerTrigger = SetTriggerParameters<QuestionViewModel>(DeepThoughtTriggers.YourQuestionToTheAnswer);
            StateMachine.Configure(DeepThoughtStates.SoLongAndThanksForAllTheFish)
                .OnEntryFrom(yourQuestionToTheAnswerTrigger, DeepThoughtTriggers.YourQuestionToTheAnswer.YourAnswer)
                .OnActivate(DeepThoughtStates.SoLongAndThanksForAllTheFish.GetQuestion);
        }       
    }
}
