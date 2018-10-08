namespace StateMachine.MultiStepForm
{
    public abstract class Trigger
    {
        public abstract string Description { get; }

        public string Key => GetType().AssemblyQualifiedName;
    }
}
