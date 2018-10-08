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
                .PermitIf(DeepThoughtTrigger.AskDeepThought, DeepThoughtStates.CorrectAnswer, DeepThoughtStates.CorrectAnswer.IsCorrectAnswer)
                .PermitIf(DeepThoughtTrigger.AskDeepThought, DeepThoughtStates.IncorrectAnswer, DeepThoughtStates.CorrectAnswer.IsNotCorrectAnswer);

            StateMachine.Configure(DeepThoughtStates.CorrectAnswer)
                .Permit(DeepThoughtTrigger.WhatIsTheQuestion, DeepThoughtStates.QuestionToTheAnswer);

            StateMachine.Configure(DeepThoughtStates.IncorrectAnswer)
                .Permit(DeepThoughtTrigger.TryAgain, DeepThoughtStates.MeaningOfLife)
                .Permit(DeepThoughtTrigger.WhatIsTheQuestion, DeepThoughtStates.QuestionToTheAnswer);

            StateMachine.Configure(DeepThoughtStates.QuestionToTheAnswer)
                .Permit(DeepThoughtTrigger.YourQuestionToTheAnswer, DeepThoughtStates.SoLongAndThanksForAllTheFish);

            var yourQuestionToTheAnswerTrigger = SetTriggerParameters<QuestionViewModel>(DeepThoughtTrigger.YourQuestionToTheAnswer);
            
            StateMachine.Configure(DeepThoughtStates.SoLongAndThanksForAllTheFish)
                .OnEntryFrom(yourQuestionToTheAnswerTrigger, DeepThoughtTrigger.YourQuestionToTheAnswer.YourAnswer)
                .OnActivate(DeepThoughtStates.SoLongAndThanksForAllTheFish.GetQuestion);
        }       
    }
}
