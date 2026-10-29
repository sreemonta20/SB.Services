namespace SBERP.Shared.Storage
{
    /// <summary>
    /// Where uploaded photos (user profile, employee) are kept. Bound from the "PhotoStorage" section of
    /// appsettings.json; override per environment with appsettings.{Environment}.json or environment
    /// variables (e.g. PhotoStorage__RootPath=/var/sberp/photos on a Linux server, or an Azure Files mount).
    /// </summary>
    public class PhotoStorageSettings
    {
        public const string SectionName = "PhotoStorage";

        /// <summary>
        /// Root folder of all photos. Empty = the OS "Pictures" folder of the account running the service
        /// (Windows: This PC\Pictures), falling back to {ContentRoot}/App_Data/Photos when there is none.
        /// </summary>
        public string? RootPath { get; set; }

        /// <summary>Sub folder under <see cref="RootPath"/> for user profile photos.</summary>
        public string UserProfileFolder { get; set; } = "userprofile";

        /// <summary>Sub folder under <see cref="RootPath"/> for employee photos.</summary>
        public string EmployeeFolder { get; set; } = "employee";

        /// <summary>Largest accepted upload, in KB.</summary>
        public int MaxFileSizeKB { get; set; } = 2048;
    }
}
