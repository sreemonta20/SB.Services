using System.Security.Cryptography;

namespace SBERP.Security.Service
{
    public interface IEncryptDecryptService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
        Stream DecryptStream(Stream cipherStream);
        CryptoStream EncryptStream(Stream responseStream);
    }
}