using Microsoft.AspNetCore.SignalR;

namespace NotificationService.Hubs
{
    // [Authorize] // يجب أن يكون المستخدم مسجلاً للدخول ليستلم إشعاراته
    //public class NotificationHub : Hub
    //{
    //    // يمكن إضافة دوال هنا إذا أردت أن يرسل الفرونت للباك، 
    //    // لكن حالياً نحن نحتاج الباك يرسل للفرونت فقط.

    //    public override async Task OnConnectedAsync()
    //    {
    //        var userId = Context.UserIdentifier;
    //        // يمكن تسجيل الاتصال هنا لأغراض التتبع
    //        await base.OnConnectedAsync();
    //    }
    //}

    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // 1. التقاط الـ ID القادم من الفرونت إند
            var httpContext = Context.GetHttpContext();
            var userId = httpContext.Request.Query["userId"].ToString().ToLower();

            // 2. إذا وجدنا ID، نضيف هذا الاتصال لمجموعة خاصة به
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
            }

            await base.OnConnectedAsync();
        }
    }
}
