namespace StateMachine.MultiStepForm
{
    public abstract class State
    {
        public string Key => GetType().AssemblyQualifiedName;
    }
}