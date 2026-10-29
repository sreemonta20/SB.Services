namespace SBERP.Security.Models.Request
{
    /// <summary>multipart/form-data body of User/uploadAppUserProfilePhoto.</summary>
    public class AppUserProfilePhotoRequest
    {
        /// <summary>AppUserProfiles.Id of the profile the photo belongs to.</summary>
        public string? Id { get; set; }
        /// <summary>JPG or PNG image, at most PhotoStorage:MaxFileSizeKB.</summary>
        public IFormFile? File { get; set; }
        /// <summary>Profile id of the user doing the upload (UpdatedBy).</summary>
        public string? CreateUpdateBy { get; set; }
    }
}
