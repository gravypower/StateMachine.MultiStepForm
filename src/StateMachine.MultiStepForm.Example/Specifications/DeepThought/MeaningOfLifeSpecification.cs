using System;
using System.Linq.Expressions;
using StateMachine.MultiStepForm.Example.Models.DeepThought;

namespace StateMachine.MultiStepForm.Example.Specifications.DeepThought
{
    public class MeaningOfLifeSpecification : Specification<AnswerViewModel>
    {
        public override Expression<Func<AnswerViewModel, bool>> ToExpression()
        {
            return question => question.Answer == "42";
        }
    }
}
