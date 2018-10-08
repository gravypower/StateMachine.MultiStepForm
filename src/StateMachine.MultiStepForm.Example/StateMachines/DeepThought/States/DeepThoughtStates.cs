using System.Collections.Generic;

namespace StateMachine.MultiStepForm.Example.StateMachines.DeepThought.States
{
    public class DeepThoughtStates : AbstractStateMachineStates
    {
        public MeaningOfLife MeaningOfLife => GetState<MeaningOfLife>();
        public CorrectAnswer CorrectAnswer => GetState<CorrectAnswer>();
        public IncorrectAnswer IncorrectAnswer => GetState<IncorrectAnswer>();
        public QuestionToTheAnswer QuestionToTheAnswer => GetState<QuestionToTheAnswer>();
        public SoLongAndThanksForAllTheFish SoLongAndThanksForAllTheFish => GetState<SoLongAndThanksForAllTheFish>();

        public DeepThoughtStates(IEnumerable<State> states): base(states)
        {
        }
    }
}
