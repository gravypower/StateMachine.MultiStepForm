using System.Collections.Generic;
using StateMachine.MultiStepForm.StateMachines.DeepThought;

namespace StateMachine.MultiStepForm.MagicStrings
{
    public interface IDeepThoughtMagicStrings: IStateMachineMagicStrings<Trigger>
    {
        
    }

    public class DeepThoughtMagicStrings: IDeepThoughtMagicStrings
    {
        public IDictionary<Trigger, string> TriggerDescriptions =>
            new Dictionary<Trigger, string>
            {
                {
                    Trigger.AskDeepThought, "Your Answer"
                },
                {
                    Trigger.TryAgain, "Try Again"
                },
                {
                    Trigger.WhatIsTheQuestion, "Lets find the Question"
                },
                {
                    Trigger.YourQuestionToTheAnswer, "The Question"
                }
            };
    }
}
