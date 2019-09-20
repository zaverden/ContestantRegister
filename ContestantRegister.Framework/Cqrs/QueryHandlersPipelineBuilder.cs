namespace ContestantRegister.Framework.Cqrs
{
    public class QueryHandlersPipelineBuilder<TQuery, TResult> : RequestPipelineBuilder<IQueryHandler<TQuery, TResult>, QueryHandlerDecorator<TQuery, TResult>> 
        where TQuery : IQuery<TResult>
    {
        
    }
}
