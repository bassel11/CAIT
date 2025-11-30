using MediatR;
using MeetingApplication.Repositories;

namespace MeetingApplication.Behaviour
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IUnitOfWork _uow;


        public TransactionBehavior(IUnitOfWork uow)
        {
            _uow = uow;
        }


        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            await _uow.BeginTransactionAsync(cancellationToken);
            try
            {
                var response = await next();
                await _uow.CommitAsync(cancellationToken);
                return response;
            }
            catch
            {
                await _uow.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
