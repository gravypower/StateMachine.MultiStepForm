using System.Collections.Generic;

namespace StateMachine.MultiStepForm.Example.StateMachines.DeepThought.Triggers
{
    public class DeepThoughtTrigger : AbstractStateMachineTriggers
    {
        public AskDeepThought AskDeepThought => GetGrigger<AskDeepThought>();
        public WhatIsTheQuestion WhatIsTheQuestion => GetGrigger<WhatIsTheQuestion>();
        public TryAgain TryAgain => GetGrigger<TryAgain>();
        public YourQuestionToTheAnswer YourQuestionToTheAnswer => GetGrigger<YourQuestionToTheAnswer>();

        public DeepThoughtTrigger(IEnumerable<Trigger> triggers) : base(triggers)
        {
        }
    }
}
