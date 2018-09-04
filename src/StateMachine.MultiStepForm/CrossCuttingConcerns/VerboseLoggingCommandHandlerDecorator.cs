using Microsoft.Extensions.Logging;
using StateMachine.MultiStepForm.Commands;

namespace StateMachine.MultiStepForm.CrossCuttingConcerns
{
    public class VerboseLoggingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : Command
    {

        private readonly ICommandHandler<TCommand> _decorated;
        private readonly ILogger<TCommand> _logger;

        public VerboseLoggingCommandHandlerDecorator(ICommandHandler<TCommand> decorated, ILogger<TCommand> logger)
        {
            _decorated = decorated;
            _logger = logger;
        }

        public void Handle(TCommand command)
        {
            _logger.LogDebug($"Handling command: {command}");

            _decorated.Handle(command);
        }
    }
}
