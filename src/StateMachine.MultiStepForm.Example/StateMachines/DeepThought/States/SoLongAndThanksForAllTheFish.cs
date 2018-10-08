using StateMachine.MultiStepForm.Contexts;
using StateMachine.MultiStepForm.Example.Models.DeepThought;
using StateMachine.MultiStepForm.Example.Queries;
using StateMachine.MultiStepForm.Example.Queries.DeepThought;

namespace StateMachine.MultiStepForm.Example.StateMachines.DeepThought.States
{
    public class SoLongAndThanksForAllTheFish : State
    {
        private readonly StateContext _stateContext;
        private readonly IQueryHandler<GetYourQuestion, string> _getYourQuestionIQueryHandler;

        public SoLongAndThanksForAllTheFish(
            StateContext stateContext,
            IQueryHandler<GetYourQuestion, string> getYourQuestionIQueryHandler)
        {
            _stateContext = stateContext;
            _getYourQuestionIQueryHandler = getYourQuestionIQueryHandler;
        }

        public void GetQuestion()
        {
            var question = _getYourQuestionIQueryHandler.Handle(new GetYourQuestion());
            _stateContext.SetModel(this,
                new QuestionViewModel
                {
                    Question = question
                });
        }
    }
}