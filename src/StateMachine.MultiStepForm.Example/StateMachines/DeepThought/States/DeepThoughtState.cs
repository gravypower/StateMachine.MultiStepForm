using System.Collections.Generic;
using System.Linq;

namespace StateMachine.MultiStepForm.Example.StateMachines.DeepThought.States
{
    public class DeepThoughtState 
    {
        private readonly IEnumerable<State> _states;

        public MeaningOfLife MeaningOfLife => _states.Single(s => s.GetType() == typeof(MeaningOfLife)) as MeaningOfLife;
        public CorrectAnswer CorrectAnswer => _states.Single(s => s.GetType() == typeof(CorrectAnswer)) as CorrectAnswer;
        public State IncorrectAnswer => _states.Single(s => s.GetType() == typeof(IncorrectAnswer));
        public State QuestionToTheAnswer => _states.Single(s => s.GetType() == typeof(QuestionToTheAnswer));
        public State SoLongAndThanksForAllTheFish => _states.Single(s => s.GetType() == typeof(SoLongAndThanksForAllTheFish));

        public DeepThoughtState(IEnumerable<State> states)
        {
            _states = states;
        }
    }
}
