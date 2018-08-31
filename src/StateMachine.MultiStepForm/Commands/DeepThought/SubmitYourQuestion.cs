using System;

namespace StateMachine.MultiStepForm.Commands.DeepThought
{
    public class SubmitYourQuestion:Command
    {
        public string Question { get; set; }
        protected override string ToHumanReadableString()
        {
            return $"your brain contained the question of \"{Question}\" to the answer of Life the universe and everything.";
        }
    }
}
