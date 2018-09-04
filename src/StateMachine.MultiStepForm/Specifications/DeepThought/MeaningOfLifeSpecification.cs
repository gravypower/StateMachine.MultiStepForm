using System;
using System.Linq.Expressions;
using StateMachine.MultiStepForm.Models.DeepThought;

namespace StateMachine.MultiStepForm.Specifications.DeepThought
{
    public class MeaningOfLifeSpecification : Specification<AnswerViewModel>
    {
        public override Expression<Func<AnswerViewModel, bool>> ToExpression()
        {
            return question => question.Answer == "42";
        }
    }
}
