using CommitteeApplication.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace CommitteeInfrastructure.Services
{
    public class PaginationService : IPaginationService
    {
        public async Task<PaginatedResult<T>> PaginateAsync<T>(
            IQueryable<T> query,
            int pageNumber,
            int pageSize)
        {
            // 1) حماية من Null Query
            if (query is null)
                return PaginatedResult<T>.Failure("Query is null");

            // 2) حماية من pageNumber/pageSize القيم الخاطئة
            if (pageNumber <= 0)
                pageNumber = 1;

            if (pageSize <= 0)
                pageSize = 10;

            // 3) العدد الكلي
            int totalCount = await query.CountAsync();

            // 4) لو لا يوجد سجلات → أرجع صفحة فارغة بدون أخطاء
            if (totalCount == 0)
                return PaginatedResult<T>.Success(new List<T>(), 0, pageNumber, pageSize);

            // 5) حساب عدد الصفحات
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // 6) لو رقم الصفحة أكبر من آخر صفحة → اجعله آخر صفحة
            //if (pageNumber > totalPages)
            //    pageNumber = totalPages;

            // 7) حساب Skip مع ضمان عدم السلبية
            int skip = (pageNumber - 1) * pageSize;

            if (skip < 0)
                skip = 0;

            // 8) تنفيذ الاستعلام
            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            // 9) النتيجة النهائية
            return PaginatedResult<T>.Success(items, totalCount, pageNumber, pageSize);
        }
    }
}

