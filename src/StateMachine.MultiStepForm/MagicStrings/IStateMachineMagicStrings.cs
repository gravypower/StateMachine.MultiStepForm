using System.Collections.Generic;

namespace StateMachine.MultiStepForm.MagicStrings
{
    public interface IStateMachineMagicStrings<TTrigger>
    {
        IDictionary<TTrigger, string> TriggerDescriptions { get; }
    }
}
