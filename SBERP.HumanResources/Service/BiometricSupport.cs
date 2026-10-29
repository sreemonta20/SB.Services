using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SBERP.HumanResources.Models.Base;
using SBERP.HumanResources.Models.Configuration;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace SBERP.HumanResources.Service
{
    /// <summary>Office clock: time zone and the attendance-day cut-off from HRSettings.</summary>
    public sealed class AttendanceClock
    {
        public TimeZoneInfo TimeZone { get; }
        public TimeSpan DayCutoff { get; }

        public AttendanceClock(HRSettings? s)
        {
            TimeZone = FindZone(s?.TimeZoneId);
            DayCutoff = TimeSpan.TryParse(s?.AttendanceDayCutoffTime, CultureInfo.InvariantCulture, out var c) ? c : TimeSpan.Zero;
        }

        public DateTime ToLocal(DateTime utc) => TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), TimeZone);

        public DateTime ToUtc(DateTime local) => TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), TimeZone);

        /// <summary>A punch before the cut-off (e.g. 02:00 with cut-off 04:00) belongs to the previous day.</summary>
        public DateTime WorkDate(DateTime local) => (local - DayCutoff).Date;

        private static TimeZoneInfo FindZone(string? id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
                catch (TimeZoneNotFoundException) { }
                catch (InvalidTimeZoneException) { }
            }
            return TimeZoneInfo.Local;
        }
    }

    /// <summary>
    /// Folder for biometric template files (development: a local folder; production: a dedicated
    /// server folder — AppSettings:Attendance:BiometricVaultPath). Every file is encrypted with
    /// ASP.NET Core Data Protection before it is written and is named after the RegistrationNo.
    /// </summary>
    public interface IBiometricVault
    {
        int MaxBytes { get; }
        Task<(string fileName, string sha256, int size)> SaveAsync(string registrationNo, byte[] content);
        Task DeleteAsync(string? fileName);
    }

    public sealed class BiometricVault : IBiometricVault
    {
        private readonly IDataProtector _protector;
        private readonly string _root;
        public int MaxBytes { get; }

        public BiometricVault(IDataProtectionProvider provider, IOptions<AppSettings> opts)
        {
            var settings = opts.Value;
            _protector = provider.CreateProtector("SBERP.HumanResources.BiometricVault.v1");
            _root = settings?.Attendance?.BiometricVaultPath ?? @"C:\SBERP\BiometricVault";
            MaxBytes = settings?.Attendance?.MaxTemplateBytes > 0 ? settings.Attendance.MaxTemplateBytes : 2 * 1024 * 1024;
        }

        public async Task<(string fileName, string sha256, int size)> SaveAsync(string registrationNo, byte[] content)
        {
            Directory.CreateDirectory(_root);
            string fileName = registrationNo + ".bio";
            byte[] cipher = _protector.Protect(content);
            await File.WriteAllBytesAsync(Path.Combine(_root, fileName), cipher);
            return (fileName, Convert.ToHexString(SHA256.HashData(cipher)), content.Length);
        }

        public Task DeleteAsync(string? fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string path = Path.Combine(_root, Path.GetFileName(fileName));
                if (File.Exists(path)) File.Delete(path);
            }
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// One-time random challenges the phone must sign. Kept in Redis for a short time and
    /// removed on first use, so a captured request cannot be replayed.
    /// </summary>
    public interface IBiometricChallengeStore
    {
        int LifetimeSeconds { get; }
        Task<(string id, string nonce)> IssueAsync(string registrationNo, string purpose);
        Task<(string registrationNo, string purpose, string nonce)?> ConsumeAsync(string? challengeId);
    }

    public sealed class BiometricChallengeStore : IBiometricChallengeStore
    {
        private readonly IDistributedCache _cache;
        public int LifetimeSeconds { get; }

        public BiometricChallengeStore(IDistributedCache cache, IOptions<AppSettings> opts)
        {
            var settings = opts.Value;
            _cache = cache;
            LifetimeSeconds = settings?.Attendance?.ChallengeSeconds > 0 ? settings.Attendance.ChallengeSeconds : 120;
        }

        private static string Key(string id) => "bio_challenge_" + id;

        public async Task<(string id, string nonce)> IssueAsync(string registrationNo, string purpose)
        {
            string id = Guid.NewGuid().ToString("N");
            string nonce = Base64Url(RandomNumberGenerator.GetBytes(32));
            string value = JsonConvert.SerializeObject(new[] { registrationNo, purpose, nonce });
            await _cache.SetStringAsync(Key(id), value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(LifetimeSeconds)
            });
            return (id, nonce);
        }

        public async Task<(string registrationNo, string purpose, string nonce)?> ConsumeAsync(string? challengeId)
        {
            if (string.IsNullOrWhiteSpace(challengeId) || challengeId.Length > 64) return null;
            string? value = await _cache.GetStringAsync(Key(challengeId));
            if (value == null) return null;
            await _cache.RemoveAsync(Key(challengeId));
            var parts = JsonConvert.DeserializeObject<string[]>(value);
            return parts is { Length: 3 } ? (parts[0], parts[1], parts[2]) : null;
        }

        private static string Base64Url(byte[] b) => Convert.ToBase64String(b).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    /// <summary>ECDSA P-256 / SHA-256 signatures made by the phone's secure hardware key.</summary>
    public static class DeviceSignature
    {
        /// <summary>The exact text the phone signs.</summary>
        public static string Message(string nonce, string registrationNo, string purpose, string? clientRequestId)
            => $"{nonce}|{registrationNo}|{purpose}|{clientRequestId}";

        public static bool IsValidPublicKey(string? publicKeyBase64)
        {
            try
            {
                using var ec = ECDsa.Create();
                ec.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKeyBase64 ?? ""), out _);
                return ec.KeySize == 256;
            }
            catch (Exception) { return false; }
        }

        /// <summary>Accepts DER (Android Keystore, iOS Secure Enclave) and raw r||s (WebCrypto) signatures.</summary>
        public static bool Verify(string? publicKeyBase64, string message, string? signatureBase64)
        {
            try
            {
                byte[] sig = Convert.FromBase64String(signatureBase64 ?? "");
                using var ec = ECDsa.Create();
                ec.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKeyBase64 ?? ""), out _);
                byte[] data = Encoding.UTF8.GetBytes(message);
                var format = sig.Length == 64 ? DSASignatureFormat.IeeeP1363FixedFieldConcatenation
                                              : DSASignatureFormat.Rfc3279DerSequence;
                return ec.VerifyData(data, sig, HashAlgorithmName.SHA256, format);
            }
            catch (Exception) { return false; }
        }
    }

    /// <summary>"Is this punch inside the office?" against the active AttendanceZones.</summary>
    public static class OfficeNetwork
    {
        public static (bool ok, string? rule) Check(IEnumerable<AttendanceZone> zones, string? clientIp,
                                                    string? bssid, decimal? lat, decimal? lng)
        {
            IPAddress? ip = null;
            if (IPAddress.TryParse(clientIp, out var parsed))
                ip = parsed.IsIPv4MappedToIPv6 ? parsed.MapToIPv4() : parsed;

            foreach (var z in zones)
            {
                // the server sees the IP itself: the strongest check
                if (ip != null && Split(z.AllowedIpRanges).Any(r => InRange(ip, r)))
                    return (true, $"IP:{z.ZoneCode}");
            }
            foreach (var z in zones)
            {
                if (!string.IsNullOrWhiteSpace(bssid) &&
                    Split(z.AllowedWifiBssids).Any(b => NormalizeMac(b) == NormalizeMac(bssid)))
                    return (true, $"WIFI:{z.ZoneCode}");
                if (lat.HasValue && lng.HasValue && z.Latitude.HasValue && z.Longitude.HasValue && z.RadiusMeters > 0 &&
                    DistanceMeters((double)lat, (double)lng, (double)z.Latitude, (double)z.Longitude) <= z.RadiusMeters)
                    return (true, $"GPS:{z.ZoneCode}");
            }
            return (false, null);
        }

        private static IEnumerable<string> Split(string? csv)
            => (csv ?? "").Split(new[] { ',', ';', ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        private static string NormalizeMac(string mac) => mac.Trim().Replace("-", ":").ToLowerInvariant();

        /// <summary>"192.168.1.0/24", "::1/128" or a single address.</summary>
        public static bool InRange(IPAddress ip, string range)
        {
            string[] parts = range.Trim().Split('/');
            if (!IPAddress.TryParse(parts[0], out var network)) return false;
            if (network.AddressFamily != ip.AddressFamily) return false;
            int bits = network.AddressFamily == AddressFamily.InterNetwork ? 32 : 128;
            int prefix = parts.Length > 1 && int.TryParse(parts[1], out var p) ? Math.Clamp(p, 0, bits) : bits;

            byte[] a = ip.GetAddressBytes(), b = network.GetAddressBytes();
            for (int i = 0; i < a.Length && prefix > 0; i++, prefix -= 8)
            {
                int mask = prefix >= 8 ? 0xFF : (0xFF << (8 - prefix)) & 0xFF;
                if ((a[i] & mask) != (b[i] & mask)) return false;
            }
            return true;
        }

        public static bool IsValidRangeList(string? csv)
            => Split(csv).All(r =>
            {
                string[] parts = r.Split('/');
                return parts.Length <= 2 && IPAddress.TryParse(parts[0], out _) &&
                       (parts.Length == 1 || int.TryParse(parts[1], out var p) && p >= 0 && p <= 128);
            });

        private static double DistanceMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000;
            double dLat = (lat2 - lat1) * Math.PI / 180, dLon = (lon2 - lon1) * Math.PI / 180;
            double h = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return 2 * R * Math.Asin(Math.Sqrt(h));
        }
    }
}
