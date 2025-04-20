using SBERP.Security.Models.Configuration;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto.IO;
using System.Security.Cryptography;
using System.Text;

namespace SBERP.Security.Service
{
    //public class EncryptDecryptService : IEncryptDecryptService
    //{
    //    private readonly AppSettings _appSettings;
    //    private readonly byte[] _key;
    //    private readonly byte[] _iv;

    //    public EncryptDecryptService(IOptions<AppSettings> appSettings)
    //    {
    //        _appSettings = appSettings.Value;
    //        _key = Encoding.UTF8.GetBytes(_appSettings.EncryptKey);
    //        _iv = Encoding.UTF8.GetBytes(_appSettings.EncryptIV);
    //    }
    //    //public EncryptDecryptService(string key, string iv)
    //    //{
    //    //    _key = Encoding.UTF8.GetBytes(key);
    //    //    _iv = Encoding.UTF8.GetBytes(iv);
    //    //}

    //    public string Encrypt(string plainText)
    //    {
    //        using var aes = Aes.Create();
    //        aes.Key = _key;
    //        aes.IV = _iv;

    //        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

    //        using var msEncrypt = new MemoryStream();
    //        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
    //        using (var swEncrypt = new StreamWriter(csEncrypt))
    //        {
    //            swEncrypt.Write(plainText);
    //        }

    //        var encrypted = msEncrypt.ToArray();

    //        return Convert.ToBase64String(encrypted);
    //    }

    //    public string Decrypt(string cipherText)
    //    {
    //        var cipherBytes = Convert.FromBase64String(cipherText);

    //        using var aes = Aes.Create();
    //        aes.Key = _key;
    //        aes.IV = _iv;

    //        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

    //        using var msDecrypt = new MemoryStream(cipherBytes);
    //        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
    //        using var srDecrypt = new StreamReader(csDecrypt);

    //        return srDecrypt.ReadToEnd();
    //    }

    //    public Stream DecryptStream(Stream cipherStream)
    //    {
    //        Aes aes = GetEncryptionAlgorithm();

    //        FromBase64Transform base64Transform = new FromBase64Transform(FromBase64TransformMode.IgnoreWhiteSpaces);
    //        CryptoStream base64DecodedStream = new CryptoStream(cipherStream, base64Transform, CryptoStreamMode.Read);
    //        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
    //        CryptoStream decryptedStream = new CryptoStream(base64DecodedStream, decryptor, CryptoStreamMode.Read);
    //        return decryptedStream;
    //    }

    //    public CryptoStream EncryptStream(Stream responseStream)
    //    {
    //        Aes aes = GetEncryptionAlgorithm();

    //        ToBase64Transform base64Transform = new ToBase64Transform();
    //        CryptoStream base64EncodedStream = new CryptoStream(responseStream, base64Transform, CryptoStreamMode.Write);
    //        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
    //        CryptoStream cryptoStream = new CryptoStream(base64EncodedStream, encryptor, CryptoStreamMode.Write);

    //        return cryptoStream;
    //    }

    //    private Aes GetEncryptionAlgorithm()
    //    {
    //        Aes aes = Aes.Create();
    //        aes.Key = _key;
    //        aes.IV = _iv;

    //        return aes;
    //    }
    //}

    public class EncryptDecryptService
    {
        
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptDecryptService(string key, string iv)
        {
            _key = Encoding.UTF8.GetBytes(key);
            _iv = Encoding.UTF8.GetBytes(iv);
        }
        //public EncryptDecryptService(string key, string iv)
        //{
        //    _key = Encoding.UTF8.GetBytes(key);
        //    _iv = Encoding.UTF8.GetBytes(iv);
        //}

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            var encrypted = msEncrypt.ToArray();

            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string cipherText)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var msDecrypt = new MemoryStream(cipherBytes);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }

        public Stream DecryptStream(Stream cipherStream)
        {
            Aes aes = GetEncryptionAlgorithm();

            FromBase64Transform base64Transform = new FromBase64Transform(FromBase64TransformMode.IgnoreWhiteSpaces);
            CryptoStream base64DecodedStream = new CryptoStream(cipherStream, base64Transform, CryptoStreamMode.Read);
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            CryptoStream decryptedStream = new CryptoStream(base64DecodedStream, decryptor, CryptoStreamMode.Read);
            return decryptedStream;
        }

        public CryptoStream EncryptStream(Stream responseStream)
        {
            Aes aes = GetEncryptionAlgorithm();

            ToBase64Transform base64Transform = new ToBase64Transform();
            CryptoStream base64EncodedStream = new CryptoStream(responseStream, base64Transform, CryptoStreamMode.Write);
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            CryptoStream cryptoStream = new CryptoStream(base64EncodedStream, encryptor, CryptoStreamMode.Write);

            return cryptoStream;
        }

        private Aes GetEncryptionAlgorithm()
        {
            Aes aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            return aes;
        }
    }
}
