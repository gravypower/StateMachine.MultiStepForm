using StateMachine.MultiStepForm.Commands;
using StateMachine.MultiStepForm.Commands.DeepThought;
using StateMachine.MultiStepForm.Models.DeepThought;
using StateMachine.MultiStepForm.Queries;
using StateMachine.MultiStepForm.Queries.DeepThought;
using StateMachine.MultiStepForm.Specifications;

namespace StateMachine.MultiStepForm.StateMachines.DeepThought
{
    public class DeepThoughtStateMachine : AbstractStateMachine<State, Trigger>
    {
        private readonly ICommandHandler<SubmitYourQuestion> _submitYourQuestionCommandHandler;
        private readonly IQueryHandler<GetYourQuestion, string> _getYourQuestionIQueryHandler;
        private readonly Specification<AnswerViewModel> _meaningOfLifeSpecification;
        public override State DefaultInitialState => State.QuestionToTheAnswer;

        public DeepThoughtStateMachine(
            ICommandHandler<SubmitYourQuestion> submitYourQuestionCommandHandler,
            IQueryHandler<GetYourQuestion, string> getYourQuestionIQueryHandler,
            Specification<AnswerViewModel> meaningOfLifeSpecification
            )
        {
            _submitYourQuestionCommandHandler = submitYourQuestionCommandHandler;
            _getYourQuestionIQueryHandler = getYourQuestionIQueryHandler;
            _meaningOfLifeSpecification = meaningOfLifeSpecification;
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
                .OnEntryFrom(yourQuestionToTheAnswerTrigger, YourAnswer)
                .OnActivate(GetQuestion);
        }

        public bool CorrectAnswer()
        {
            var arg = GetArg<AnswerViewModel>(Trigger.AskDeepThought);
            return _meaningOfLifeSpecification.IsSatisfiedBy(arg);
        }

        private void YourAnswer(QuestionViewModel question)
        {
            var command = new SubmitYourQuestion
            {
                Question = question.Question
            };

            _submitYourQuestionCommandHandler.Handle(command);
        }
        
        private void GetQuestion()
        {
            var question = _getYourQuestionIQueryHandler.Handle(new GetYourQuestion());
            SetModel(State.SoLongAndThanksForAllTheFish,
                new QuestionViewModel
                {
                    Question = question
                });
        }
    }
}
