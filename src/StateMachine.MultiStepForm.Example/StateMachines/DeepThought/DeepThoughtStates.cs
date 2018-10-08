using StateMachine.MultiStepForm.Example.StateMachines.DeepThought.States;

namespace StateMachine.MultiStepForm.Example.StateMachines.DeepThought
{
    public static class DeepThoughtStates
    {
        public static MeaningOfLife MeaningOfLife => Startup.Container.GetInstance<MeaningOfLife>();
        public static CorrectAnswer CorrectAnswer => Startup.Container.GetInstance<CorrectAnswer>();
        public static IncorrectAnswer IncorrectAnswer => Startup.Container.GetInstance<IncorrectAnswer>();
        public static QuestionToTheAnswer QuestionToTheAnswer => Startup.Container.GetInstance<QuestionToTheAnswer>();
        public static SoLongAndThanksForAllTheFish SoLongAndThanksForAllTheFish => Startup.Container.GetInstance<SoLongAndThanksForAllTheFish>();

        public abstract class State : MultiStepForm.State
        {
        }
    }
}
