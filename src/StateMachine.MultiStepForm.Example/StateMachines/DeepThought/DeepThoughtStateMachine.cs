using System.Collections.Generic;
using StateMachine.MultiStepForm.Contexts;
using StateMachine.MultiStepForm.Example.Commands;
using StateMachine.MultiStepForm.Example.Commands.DeepThought;
using StateMachine.MultiStepForm.Example.Models.DeepThought;
using StateMachine.MultiStepForm.Example.Queries;
using StateMachine.MultiStepForm.Example.Queries.DeepThought;
using StateMachine.MultiStepForm.Example.Specifications;
using StateMachine.MultiStepForm.Example.StateMachines.DeepThought.States;
using StateMachine.MultiStepForm.Example.StateMachines.DeepThought.Triggers;

namespace StateMachine.MultiStepForm.Example.StateMachines.DeepThought
{
    public class DeepThoughtStateMachine : AbstractStateMachine<State, Trigger>
    {
        private readonly DeepThoughtState _states;
        private readonly DeepThoughtTrigger _triggers;
        private readonly ICommandHandler<SubmitYourQuestion> _submitYourQuestionCommandHandler;
        private readonly IQueryHandler<GetYourQuestion, string> _getYourQuestionIQueryHandler;

        public override State DefaultInitialState => _states.MeaningOfLife;

        public DeepThoughtStateMachine(
            StateContext stateContext,
            TriggerContext triggerContext,
            DeepThoughtState states,
            DeepThoughtTrigger triggers,
            ICommandHandler<SubmitYourQuestion> submitYourQuestionCommandHandler,
            IQueryHandler<GetYourQuestion, string> getYourQuestionIQueryHandler) : base(triggerContext, stateContext)
        {
            _states = states;
            _triggers = triggers;
            _submitYourQuestionCommandHandler = submitYourQuestionCommandHandler;
            _getYourQuestionIQueryHandler = getYourQuestionIQueryHandler;
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
                .OnEntryFrom(yourQuestionToTheAnswerTrigger, YourAnswer)
                .OnActivate(GetQuestion);
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
            StateContext.SetModel(_states.SoLongAndThanksForAllTheFish,
                new QuestionViewModel
                {
                    Question = question
                });
        }
    }
}
