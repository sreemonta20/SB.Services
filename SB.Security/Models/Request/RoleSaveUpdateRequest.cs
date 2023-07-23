namespace SB.Security.Models.Request
{
    /// <summary>
    /// UserRegisterRequest is extension of  <see cref="UserRole"/>.
    /// </summary>
    public class RoleSaveUpdateRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public string? CreateUpdateBy { get; set; }
        public Boolean? IsActive { get; set; }
    }
}
