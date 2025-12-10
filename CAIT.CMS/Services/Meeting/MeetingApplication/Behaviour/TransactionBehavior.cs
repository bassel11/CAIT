using MediatR;
using MeetingApplication.Interfaces;

namespace MeetingApplication.Behaviour
{

    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull

    {
        private readonly IUnitOfWork _uow;

        public TransactionBehavior(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {



            if (request is ITransactionalRequest)
            {
                // 1) Execute handler
                var response = await next();

                // 2) Persist changes. EF Core will open the DB transaction as needed.
                // MassTransit EF Outbox will capture any publishes done in the handler
                // and store them in Outbox within the same SaveChanges transaction.
                await _uow.SaveChangesAsync(cancellationToken);

                return response;
            }
            return await next();

            //// فقط إذا كان الطلب يطبق ITransactionalRequest
            //if (request is ITransactionalRequest)
            //{
            //    await _uow.BeginTransactionAsync(cancellationToken);
            //    try
            //    {
            //        var response = await next();
            //        await _uow.CommitAsync(cancellationToken);
            //        return response;
            //    }
            //    catch
            //    {
            //        await _uow.RollbackAsync(cancellationToken);
            //        throw;
            //    }
            //}

            // غير ذلك: مرر مباشرة بدون Transaction
            //return await next();
        }
    }
}
