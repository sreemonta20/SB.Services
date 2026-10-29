namespace SBERP.Shared.Storage
{
    /// <summary>Result of <see cref="IPhotoStorage.SaveAsync"/>.</summary>
    /// <param name="PhotoId">New id of the photo; also the stored file name (without extension).</param>
    /// <param name="Location">Path relative to the storage root, e.g. "userprofile/{PhotoId}.jpg". Saved in the DB.</param>
    public record StoredPhoto(Guid PhotoId, string Location);

    /// <summary>A stored photo opened for download. Dispose <see cref="Content"/> (the MVC File result does).</summary>
    public record PhotoFile(Stream Content, string ContentType, Guid PhotoId);

    /// <summary>
    /// Saves, reads and deletes photos. The DB keeps only the photo id and the RELATIVE location, so the
    /// root folder can change between machines / environments without touching data. Swap the
    /// implementation (Azure Blob, S3 ...) here without changing the services that use it.
    /// </summary>
    public interface IPhotoStorage
    {
        /// <summary>
        /// Validates the upload (size, real JPEG/PNG content - not just the extension) and saves it as
        /// {folder}/{new Guid}.{jpg|png}. The client's file name is never used.
        /// </summary>
        /// <exception cref="PhotoValidationException">The file is empty, too large or not a JPEG/PNG image.</exception>
        Task<StoredPhoto> SaveAsync(string folder, Stream content, long length, CancellationToken ct = default);

        /// <summary>Opens a stored photo for reading, or null when it no longer exists.</summary>
        Stream? OpenRead(string? location);

        /// <summary>Deletes a stored photo; a missing file is ignored.</summary>
        void Delete(string? location);

        /// <summary>MIME type for a stored location (image/jpeg or image/png).</summary>
        string GetContentType(string location);
    }

    /// <summary>The uploaded file is not acceptable; the message is safe to show to the user.</summary>
    public class PhotoValidationException(string message) : Exception(message);
}
