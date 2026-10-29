using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;

namespace SBERP.Security.Models.Base
{
    /// <summary>
    /// This is base user class used for getting user, user list, registerng, updating, and deleting the user.
    /// </summary>
    public class AppUserProfile
    {
        [Key]
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public Guid? AppUserRoleId { get; set; }
        /// <summary>
        /// The person's role name / designation / position shown beside the name (e.g. "Chief Executive Officer").
        /// Free text, NOT the security role (AppUserRoleId / AppUserRoles.RoleName).
        /// </summary>
        public string? JobTitle { get; set; }
        /// <summary>Soft link to HumanResourcesDB.Employees.Id for logins provisioned on onboarding (null for manual users).</summary>
        public Guid? EmployeeId { get; set; }
        /// <summary>Id of the uploaded profile photo (new Guid on every upload); also the stored file name. Null = default avatar.</summary>
        public Guid? ProfilePhotoId { get; set; }
        /// <summary>Photo path relative to PhotoStorage:RootPath, e.g. "userprofile/{ProfilePhotoId}.jpg".</summary>
        public string? ProfilePhotoLocation { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public virtual AppUserRole? AppUserRole { get; set; }
        public virtual AppUser? AppUser { get; set; }
    }
}
