namespace StateMachine.MultiStepForm.Commands
{
    public abstract class Command : ICommand
    {
        public override string ToString()
        {
            return ToHumanReadableString();
        }

        protected abstract string ToHumanReadableString();
    }
}
