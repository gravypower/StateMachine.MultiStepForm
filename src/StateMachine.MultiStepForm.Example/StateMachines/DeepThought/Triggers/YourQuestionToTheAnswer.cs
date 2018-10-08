using StateMachine.MultiStepForm.Example.Commands;
using StateMachine.MultiStepForm.Example.Commands.DeepThought;
using StateMachine.MultiStepForm.Example.Models.DeepThought;

namespace StateMachine.MultiStepForm.Example.StateMachines.DeepThought.Triggers
{
    public class YourQuestionToTheAnswer : Trigger
    {
        private readonly ICommandHandler<SubmitYourQuestion> _submitYourQuestionCommandHandler;
        public override string Description => "YourQuestionToTheAnswer";

        public YourQuestionToTheAnswer(ICommandHandler<SubmitYourQuestion> submitYourQuestionCommandHandler)
        {
            _submitYourQuestionCommandHandler = submitYourQuestionCommandHandler;
        }

        public void YourAnswer(QuestionViewModel question)
        {
            var command = new SubmitYourQuestion
            {
                Question = question.Question
            };

            _submitYourQuestionCommandHandler.Handle(command);
        }
    }
}