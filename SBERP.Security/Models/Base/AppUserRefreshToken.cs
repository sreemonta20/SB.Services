using System.ComponentModel.DataAnnotations;

namespace SBERP.Security.Models.Base
{
    /// <summary>
    /// One refresh token issued to a user. Only the SHA-256 hash of the token is stored, so a copy of
    /// the table cannot be replayed. Each login starts a new <see cref="FamilyId"/> (one browser tab or
    /// device = one family); every refresh revokes the presented token and issues its successor in the
    /// same family. Presenting a token that was already rotated means it was copied, so the whole
    /// family is revoked (reuse detection).
    /// </summary>
    public class AppUserRefreshToken
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AppUserId { get; set; }
        /// <summary>SHA-256 of the raw token, upper-case hex (64 chars).</summary>
        public string TokenHash { get; set; } = string.Empty;
        /// <summary>Login session the token belongs to; shared by all rotations of that login.</summary>
        public Guid FamilyId { get; set; }
        /// <summary>Sliding expiry: AppSettings:RefreshTokenExpirationDays after this token was issued.</summary>
        public DateTime ExpiresAt { get; set; }
        /// <summary>Absolute expiry of the login session: AppSettings:RefreshTokenSessionLifetimeDays after login. Never extended by refreshes.</summary>
        public DateTime SessionExpiresAt { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedByIp { get; set; }
        public string? UserAgent { get; set; }
        public DateTime? RevokedDate { get; set; }
        public string? RevokedByIp { get; set; }
        /// <summary>Rotated, Logout, ReuseDetected, PasswordChanged, UserDeactivated, RoleChanged, UserDeleted, Offboarded.</summary>
        public string? RevokedReason { get; set; }
        public string? ReplacedByTokenHash { get; set; }
        public virtual AppUser? AppUser { get; set; }
    }
}
