namespace ContestantRegister.Framework.Cqrs
{
    public class CommandHandlersPipelineBuilder<TCommand> : RequestPipelineBuilder<ICommandHandler<TCommand>, CommandHandlerDecorator<TCommand>> 
        where TCommand : ICommand
    {
        
    }

}
