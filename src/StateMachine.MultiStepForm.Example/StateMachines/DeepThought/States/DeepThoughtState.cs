using System.Collections.Generic;
using System.Linq;

namespace StateMachine.MultiStepForm.Example.StateMachines.DeepThought.States
{
    public class DeepThoughtState 
    {
        private readonly IEnumerable<State> _states;

        public State MeaningOfLife => _states.Single(s => s.GetType() == typeof(MeaningOfLife));
        public State CorrectAnswer => _states.Single(s => s.GetType() == typeof(CorrectAnswer));
        public State IncorrectAnswer => _states.Single(s => s.GetType() == typeof(IncorrectAnswer));
        public State QuestionToTheAnswer => _states.Single(s => s.GetType() == typeof(QuestionToTheAnswer));
        public State SoLongAndThanksForAllTheFish => _states.Single(s => s.GetType() == typeof(SoLongAndThanksForAllTheFish));

        public DeepThoughtState(IEnumerable<State> states)
        {
            _states = states;
        }
    }
}
