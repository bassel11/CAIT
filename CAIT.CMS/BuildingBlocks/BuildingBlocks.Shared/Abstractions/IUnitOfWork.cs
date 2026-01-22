namespace BuildingBlocks.Shared.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        // هذه الدالة هي المسؤولة عن الـ Transaction
        // ستقوم بحفظ تغييرات الكيانات + رسائل الـ Outbox في عملية واحدة
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
