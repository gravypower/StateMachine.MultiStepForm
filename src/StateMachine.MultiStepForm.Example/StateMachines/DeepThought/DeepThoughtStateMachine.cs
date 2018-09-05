using StateMachine.MultiStepForm.Example.Commands;
using StateMachine.MultiStepForm.Example.Commands.DeepThought;
using StateMachine.MultiStepForm.Example.Models.DeepThought;
using StateMachine.MultiStepForm.Example.Queries;
using StateMachine.MultiStepForm.Example.Queries.DeepThought;
using StateMachine.MultiStepForm.Example.Specifications;

namespace StateMachine.MultiStepForm.Example.StateMachines.DeepThought
{
    public class DeepThoughtStateMachine : AbstractStateMachine<State, Trigger>
    {
        private readonly ICommandHandler<SubmitYourQuestion> _submitYourQuestionCommandHandler;
        private readonly IQueryHandler<GetYourQuestion, string> _getYourQuestionIQueryHandler;
        private readonly Specification<AnswerViewModel> _meaningOfLifeSpecification;

        public override State DefaultInitialState => State.MeaningOfLife;

        public DeepThoughtStateMachine(
            ICommandHandler<SubmitYourQuestion> submitYourQuestionCommandHandler,
            IQueryHandler<GetYourQuestion, string> getYourQuestionIQueryHandler,
            Specification<AnswerViewModel> meaningOfLifeSpecification)
        {
            _submitYourQuestionCommandHandler = submitYourQuestionCommandHandler;
            _getYourQuestionIQueryHandler = getYourQuestionIQueryHandler;
            _meaningOfLifeSpecification = meaningOfLifeSpecification;
        }

        protected override void DoConfigureStateMachine()
        {
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

            var yourQuestionToTheAnswerTrigger = SetTriggerParameters<QuestionViewModel>(Trigger.YourQuestionToTheAnswer);
            
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
