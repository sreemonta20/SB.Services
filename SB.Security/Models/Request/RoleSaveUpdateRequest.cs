namespace SB.Security.Models.Request
{
    /// <summary>
    /// UserRegisterRequest is extension of  <see cref="UserRole"/>.
    /// </summary>
    public class RoleSaveUpdateRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean? IsActive { get; set; }
    }
}
