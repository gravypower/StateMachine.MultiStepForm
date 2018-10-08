
using StateMachine.MultiStepForm.Example.StateMachines.DeepThought.Triggers;

namespace StateMachine.MultiStepForm.Example.StateMachines.DeepThought
{
    public static class DeepThoughtTriggers
    {
        public static AskDeepThought AskDeepThought => Startup.Container.GetInstance<AskDeepThought>();
        public static WhatIsTheQuestion WhatIsTheQuestion => Startup.Container.GetInstance<WhatIsTheQuestion>();
        public static TryAgain TryAgain => Startup.Container.GetInstance<TryAgain>();
        public static YourQuestionToTheAnswer YourQuestionToTheAnswer => Startup.Container.GetInstance<YourQuestionToTheAnswer>();
    }
}
