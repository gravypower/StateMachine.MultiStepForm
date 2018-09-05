namespace StateMachine.MultiStepForm.Example.Commands
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
