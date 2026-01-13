using MassTransit;

namespace Monitoring.Application.Consumers
{
    public class UserLoggedOutConsumerDefinition : ConsumerDefinition<UserLoggedOutConsumer>
    {
        public UserLoggedOutConsumerDefinition()
        {
            // ✅ تسمية الطابور بشكل صريح ومحدد
            // التسمية: [اسم-الخدمة]-[الحدث]-[الغرض]
            // هذا يمنع تضارب الأسماء في RabbitMQ إذا كانت هناك خدمات أخرى تستمع لنفس الحدث
            EndpointName = "monitoring-user-logged-out";

            // تحديد عدد الرسائل التي تتم معالجتها في وقت واحد (اختياري، لكن مفيد للأداء مع Redis)
            ConcurrentMessageLimit = 10;
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<UserLoggedOutConsumer> consumerConfigurator,
            IRegistrationContext context)
        {
            // ✅ سياسة إعادة المحاولة (Retry Policy)
            // بما أن العملية هي "حذف كاش" (عملية حرجة للأمان ولكنها تعتمد على الشبكة/Redis)
            // نستخدم سياسة متدرجة لضمان النجاح حتى لو كان Redis مشغولاً لحظياً
            endpointConfigurator.UseMessageRetry(r =>
            {
                // حاول 5 مرات، بفاصل ثانيتين بين كل محاولة
                // زدنا العدد قليلاً لأن العملية أمنية (Logout) ومهمة جداً
                r.Interval(5, TimeSpan.FromSeconds(2));

                // تجاهل أخطاء معينة إذا كنت تعلم أنها لن تُحل (مثل خطأ في بنية البيانات)
                // r.Ignore<ArgumentNullException>(); 
            });

            // خيارات إضافية متقدمة (اختيارية):
            // endpointConfigurator.UseInMemoryOutbox(context); // لضمان عدم ضياع الرسالة إذا فشل الحفظ
        }
    }

}
