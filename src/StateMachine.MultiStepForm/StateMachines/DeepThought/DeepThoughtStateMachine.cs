using StateMachine.MultiStepForm.Commands;
using StateMachine.MultiStepForm.Commands.DeepThought;
using StateMachine.MultiStepForm.Models.DeepThought;

namespace StateMachine.MultiStepForm.StateMachines.DeepThought
{
    public class DeepThoughtStateMachine : AbstractStateMachine<State, Trigger>
    {
        private readonly ICommandHandler<SubmitYourQuestion> _submitYourQuestionCommandHandler;
        public override State DefaultInitialState => State.MeaningOfLife;

        public DeepThoughtStateMachine(ICommandHandler<SubmitYourQuestion> submitYourQuestionCommandHandler)
        {
            _submitYourQuestionCommandHandler = submitYourQuestionCommandHandler;
        }

        protected override void DoConfigureStateMachine()
        {
            var yourQuestionToTheAnswerTrigger = StateMachine.SetTriggerParameters<QuestionViewModel>(Trigger.YourQuestionToTheAnswer);
            TriggersWithParameters.Add(yourQuestionToTheAnswerTrigger);

            StateMachine.Configure(State.MeaningOfLife)
                .PermitIf(Trigger.AskDeepThought, State.CorrectAnswer, CorrectAnswer)
                .PermitIf(Trigger.AskDeepThought, State.IncorrectAnswer, () => !CorrectAnswer());

            StateMachine.Configure(State.CorrectAnswer)
                .Permit(Trigger.WhatIsTheQuestion, State.QuestionToTheAnswer);

            StateMachine.Configure(State.IncorrectAnswer)
                .Permit(Trigger.TryAgain, State.MeaningOfLife)
                .Permit(Trigger.WhatIsTheQuestion, State.QuestionToTheAnswer);

            StateMachine.Configure(State.QuestionToTheAnswer)
                .Permit(Trigger.YourQuestionToTheAnswer, State.SoLongAndThanksForAllTheFish);

            StateMachine.Configure(State.SoLongAndThanksForAllTheFish)
                .OnEntryFrom(yourQuestionToTheAnswerTrigger, YourAnswer);
        }

        public bool CorrectAnswer()
        {
            var arg = GetArg<AnswerViewModel>(Trigger.AskDeepThought);
            if (arg == null)
            {
                return false;
            }

            return arg.Answer == "42";
        }

        private void YourAnswer(QuestionViewModel question)
        {
            var command = new SubmitYourQuestion
            {
                Question = question.Question
            };

            _submitYourQuestionCommandHandler.Handle(command);
        }
    }
}
