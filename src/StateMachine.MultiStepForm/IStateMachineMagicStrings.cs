using System.Collections.Generic;

namespace StateMachine.MultiStepForm
{
    public interface IStateMachineMagicStrings<TTrigger>
    {
        IDictionary<TTrigger, string> TriggerDescriptions { get; }
    }
}
