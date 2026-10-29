using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SBERP.Shared.Storage
{
    /// <summary>
    /// <see cref="IPhotoStorage"/> on the local / mounted file system under <see cref="PhotoStorageSettings.RootPath"/>.
    /// Folders are created on first use.
    /// </summary>
    public class LocalPhotoStorage : IPhotoStorage
    {
        private static readonly byte[] JpegSignature = [0xFF, 0xD8, 0xFF];
        private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

        private readonly PhotoStorageSettings _settings;
        private readonly string _root;

        public LocalPhotoStorage(IOptions<PhotoStorageSettings> options, IHostEnvironment env, ILogger<LocalPhotoStorage> logger)
        {
            _settings = options.Value;
            _root = Path.GetFullPath(ResolveRoot(_settings.RootPath, env.ContentRootPath));
            logger.LogInformation("Photo storage root: {PhotoRoot}", _root);
        }

        /// <summary>Configured root, else the OS Pictures folder, else {contentRoot}/App_Data/Photos.</summary>
        private static string ResolveRoot(string? configured, string contentRoot)
        {
            if (!string.IsNullOrWhiteSpace(configured))
                return Environment.ExpandEnvironmentVariables(configured);
            string pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            return string.IsNullOrWhiteSpace(pictures) ? Path.Combine(contentRoot, "App_Data", "Photos") : pictures;
        }

        public async Task<StoredPhoto> SaveAsync(string folder, Stream content, long length, CancellationToken ct = default)
        {
            long maxBytes = _settings.MaxFileSizeKB * 1024L;
            if (length <= 0)
                throw new PhotoValidationException("The photo file is empty.");
            if (length > maxBytes)
                throw new PhotoValidationException($"The photo is larger than {_settings.MaxFileSizeKB} KB.");

            // the extension comes from the file content, never from the client's file name
            var buffer = new MemoryStream();
            await content.CopyToAsync(buffer, ct);
            if (buffer.Length > maxBytes)
                throw new PhotoValidationException($"The photo is larger than {_settings.MaxFileSizeKB} KB.");
            byte[] bytes = buffer.ToArray();
            string extension = StartsWith(bytes, JpegSignature) ? ".jpg"
                             : StartsWith(bytes, PngSignature) ? ".png"
                             : throw new PhotoValidationException("Only JPG and PNG images are allowed.");

            var photoId = Guid.NewGuid();
            string location = $"{SafeFolder(folder)}/{photoId:D}{extension}";
            string fullPath = FullPath(location);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            await File.WriteAllBytesAsync(fullPath, bytes, ct);
            return new StoredPhoto(photoId, location);
        }

        public Stream? OpenRead(string? location)
        {
            if (string.IsNullOrWhiteSpace(location)) return null;
            string fullPath = FullPath(location);
            return File.Exists(fullPath)
                ? new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                : null;
        }

        public void Delete(string? location)
        {
            if (string.IsNullOrWhiteSpace(location)) return;
            string fullPath = FullPath(location);
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }

        public string GetContentType(string location) =>
            location.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? "image/png" : "image/jpeg";

        /// <summary>Full path of a stored location; refuses anything that would leave the root folder.</summary>
        private string FullPath(string location)
        {
            string fullPath = Path.GetFullPath(Path.Combine(_root, location.Replace('/', Path.DirectorySeparatorChar)));
            string rootWithSeparator = _root.EndsWith(Path.DirectorySeparatorChar) ? _root : _root + Path.DirectorySeparatorChar;
            if (!fullPath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Photo location is outside the photo storage root.");
            return fullPath;
        }

        private static string SafeFolder(string folder)
        {
            string name = (folder ?? string.Empty).Trim().Trim('/', '\\');
            if (name.Length == 0 || name.Contains("..") || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new InvalidOperationException($"Invalid photo folder '{folder}'.");
            return name;
        }

        private static bool StartsWith(byte[] data, byte[] signature) =>
            data.Length >= signature.Length && data.AsSpan(0, signature.Length).SequenceEqual(signature);
    }
}
