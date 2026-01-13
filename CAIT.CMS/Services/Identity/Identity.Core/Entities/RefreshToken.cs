using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Core.Entities
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;

        // التوكن الفعلي (يجب أن يكون مفهرس Index في قاعدة البيانات للأداء)
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

        // ---------------------------------------------------------
        // ✅ Audit & Security Fields (أفضل الممارسات)
        // ---------------------------------------------------------

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // لتسجيل عنوان IP الذي طلب التوكن (مهم للأمان)
        public string CreatedByIp { get; set; } = "Unknown";

        public DateTime? RevokedAt { get; set; }

        // لتسجيل عنوان IP الذي قام بتسجيل الخروج
        public string? RevokedByIp { get; set; }

        // لدعم Token Rotation: التوكن الجديد الذي حل محل هذا التوكن
        public string? ReplacedByToken { get; set; }

        // سبب الإبطال (Logout, Security, Replay Attack, etc.)
        public string? ReasonRevoked { get; set; }

        // ---------------------------------------------------------
        // ✅ Status Flags
        // ---------------------------------------------------------

        // هل تم استخدام هذا التوكن سابقاً لتوليد Access Token؟ 
        // (مهم جداً لمنع Replay Attacks)
        public bool IsUsed { get; set; } = false;

        // هل تم إبطاله يدوياً (Logout)؟
        public bool Revoked { get; set; } = false;

        // ---------------------------------------------------------
        // ✅ Computed Properties (Helpers)
        // ---------------------------------------------------------
        [NotMapped]
        public bool IsActive => !Revoked && !IsExpired;

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
    }
}