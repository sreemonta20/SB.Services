namespace SBERP.HumanResources.Models.Request
{
    /// <summary>multipart/form-data body of Employee/uploadEmployeePhoto.</summary>
    public class EmployeePhotoRequest
    {
        /// <summary>Employees.Id of the employee the photo belongs to.</summary>
        public string? Id { get; set; }
        /// <summary>JPG or PNG image, at most PhotoStorage:MaxFileSizeKB.</summary>
        public IFormFile? File { get; set; }
        /// <summary>Profile id of the user doing the upload (UpdatedBy).</summary>
        public string? CreateUpdateBy { get; set; }
    }
}
