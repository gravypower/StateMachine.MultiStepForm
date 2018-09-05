using System.Collections.Generic;
using StateMachine.MultiStepForm.Example.StateMachines.DeepThought;

namespace StateMachine.MultiStepForm.Example.MagicStrings
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
