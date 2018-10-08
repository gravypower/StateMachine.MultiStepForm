using System.Collections.Generic;
using System.Linq;

namespace StateMachine.MultiStepForm
{
    public abstract class AbstractStateMachineTriggers
    {
        protected readonly IEnumerable<Trigger> Triggers;

        protected AbstractStateMachineTriggers(IEnumerable<Trigger> triggers)
        {
            Triggers = triggers;
        }

        protected TTrigger GetGrigger<TTrigger>()
            where TTrigger : Trigger
        {
            return Triggers.Single(s => s.GetType() == typeof(TTrigger)) as TTrigger;
        }
    }
}
