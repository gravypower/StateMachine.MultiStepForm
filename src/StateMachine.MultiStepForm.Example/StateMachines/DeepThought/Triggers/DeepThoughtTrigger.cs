using System.Collections.Generic;
using System.Linq;

namespace StateMachine.MultiStepForm.Example.StateMachines.DeepThought.Triggers
{
    public class DeepThoughtTrigger
    {
        private readonly IEnumerable<Trigger> _triggers;

        public Trigger AskDeepThought => _triggers.Single(t => t.GetType() == typeof(AskDeepThought));
        public Trigger WhatIsTheQuestion => _triggers.Single(t => t.GetType() == typeof(WhatIsTheQuestion));
        public Trigger TryAgain => _triggers.Single(t => t.GetType() == typeof(TryAgain));
        public Trigger YourQuestionToTheAnswer => _triggers.Single(t => t.GetType() == typeof(YourQuestionToTheAnswer));

        public DeepThoughtTrigger(IEnumerable<Trigger> triggers)
        {
            _triggers = triggers;
        }
    }
}
