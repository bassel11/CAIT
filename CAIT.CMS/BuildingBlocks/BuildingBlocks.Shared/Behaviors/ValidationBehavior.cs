using BuildingBlocks.Shared.CQRS;
using FluentValidation;
using MediatR;
using ValidationException = BuildingBlocks.Shared.Exceptions.ValidationException; // ✅ استخدام استثنائنا المخصص

namespace BuildingBlocks.Shared.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // 1. إذا لم يكن هناك مدققون، اكمل العملية
            if (!validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            // 2. تنفيذ جميع المدققات
            var validationResults =
                await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // 3. تجميع الأخطاء
            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            // 4. إذا وجدت أخطاء، قم بتحويلها للشكل الذي يتوقعه استثنائنا المخصص
            if (failures.Count != 0)
            {
                // تحويل القائمة إلى Dictionary<string, string[]>
                // مثال: "Title": ["Required", "Max length 50"]
                var errors = failures
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

                // 🛑 رمي الاستثناء المخصص الذي يفهمه GlobalExceptionHandler
                throw new ValidationException(errors);
            }

            return await next();
        }
    }
}