using System.Collections.Generic;
using StateMachine.MultiStepForm.StateMachines.DeepThought;

namespace StateMachine.MultiStepForm.MagicStrings
{
    public interface IDeepThoughtMagicStrings
    {
        IDictionary<string, string> TriggerDescriptions { get; }
    }

    public class DeepThoughtMagicStrings: IDeepThoughtMagicStrings
    {
        public IDictionary<string, string> TriggerDescriptions =>
            new Dictionary<string, string>
            {
                {
                    $"{Trigger.AskDeepThought}", "Your Answer"
                },
                {
                    $"{Trigger.TryAgain}", "Try Again"
                },
                {
                    $"{Trigger.WhatIsTheQuestion}", "Lets find the Question"
                },
                {
                    $"{Trigger.YourQuestionToTheAnswer}", "The Question"
                }
            };
    }
}
