using SB.Security.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace SB.Security.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class EncDecMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public EncDecMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task InvokeAsync(HttpContext? httpContext)
        {
            List<string> excludeURL = GetExcludeURLList();
            if (!excludeURL.Contains(httpContext.Request.Path.Value))
            {
                httpContext.Request.Body = DecryptStream(httpContext.Request.Body);
                if (httpContext.Request.QueryString.HasValue)
                {
                    string decryptedString = DecryptString(httpContext.Request.QueryString.Value.Substring(1));
                    httpContext.Request.QueryString = new QueryString($"?{decryptedString}");
                }
            }
            await _next(httpContext);
        }


        //public async Task InvokeAsync(HttpContext httpContext)
        //{
        //    httpContext.Request.EnableBuffering();
        //    if (httpContext.Request.Path.StartsWithSegments("/api/User/login") && httpContext.Request.Method == "POST")
        //    {
        //        string encryptedRequest = string.Empty;
        //        using (var reader = new StreamReader(httpContext.Request.Body))
        //        {
        //            encryptedRequest = await reader.ReadToEndAsync();//.ReadToEndAsync() //reader.ReadToEnd()
        //        }

        //        string decryptedRequest = await DecryptObj(encryptedRequest);
        //        httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(decryptedRequest));

        //        httpContext.Request.ContentLength = decryptedRequest.Length;
        //        httpContext.Request.ContentType = "application/json; charset=utf-8";

        //    }
        //    await _next.Invoke(httpContext);
        //}

        //public async Task InvokeAsync(HttpContext context)
        //{
        //    if (context.Request.Path.StartsWithSegments("/api/User/login") && context.Request.Method == "POST")
        //    {
        //        var request = context.Request;
        //        var stream = request.Body;// At the begining it holding original request stream                    
        //        var originalReader = new StreamReader(stream);
        //        var originalContent = await originalReader.ReadToEndAsync(); // Reading first request

        //        var decryptedRequest = await DecryptObj(originalContent);
        //        var readingRequestBody = new RequstBodyReaderModel();
        //        readingRequestBody.RequestRawData = decryptedRequest;
        //        var json = JsonConvert.SerializeObject(readingRequestBody);
        //        var requestData = Encoding.UTF8.GetBytes(json);
        //        stream = new MemoryStream(requestData);
        //        request.Body = stream;
        //    }

        //    await _next(context);
        //}

        private Stream DecryptStream(Stream cipherStream)
        {
            Aes aes = GetEncryptionAlgorithm();
            FromBase64Transform base64Transform = new FromBase64Transform(FromBase64TransformMode.IgnoreWhiteSpaces);
            CryptoStream base64DecodedStream = new CryptoStream(cipherStream, base64Transform, CryptoStreamMode.Read);
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            CryptoStream decryptedStream = new CryptoStream(base64DecodedStream, decryptor, CryptoStreamMode.Read);
            return decryptedStream;
        }
        private string DecryptString(string cipherText)
        {
            Aes aes = GetEncryptionAlgorithm();
            byte[] buffer = Convert.FromBase64String(cipherText);
            MemoryStream memoryStream = new MemoryStream(buffer);
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            StreamReader streamReader = new StreamReader(cryptoStream);
            return streamReader.ReadToEnd();
        }

        public async Task<string> DecryptObj(string decStr)
        {
            Aes aes = GetEncryptionAlgorithm();
            byte[] buffer = Convert.FromBase64String(decStr);
            MemoryStream memoryStream = new MemoryStream(buffer);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            StreamReader streamReader = new StreamReader(cryptoStream);
            return await streamReader.ReadToEndAsync();
        }



        public async Task<string> DecryptNew(string cipherText)
        {
            string result;
            string encryptionKey = "1203199320052021";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(16);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        await cs.WriteAsync(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    result = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return result;
        }


        public string Decryptword(string DecryptText)
        {
            byte[] SrctArray;
            byte[] DrctArray = Convert.FromBase64String(DecryptText);
            SrctArray = UTF8Encoding.UTF8.GetBytes("1203199320052021");
            TripleDESCryptoServiceProvider objt = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider objmdcript = new MD5CryptoServiceProvider();
            SrctArray = objmdcript.ComputeHash(UTF8Encoding.UTF8.GetBytes("1203199320052021"));
            objmdcript.Clear();
            objt.Key = SrctArray;
            //objt.KeySize = 128 / 8;
            objt.Mode = CipherMode.CBC;
            objt.Padding = PaddingMode.PKCS7;
            ICryptoTransform crptotrns = objt.CreateDecryptor();
            byte[] resArray = crptotrns.TransformFinalBlock(DrctArray, 0, DrctArray.Length);
            objt.Clear();
            return UTF8Encoding.UTF8.GetString(resArray);
        }

        public string decryptStr(string text)
        {
            string plaintext = null;
            var key = "1203199320052021";
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(text);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(key);
                ICryptoTransform decrypter = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)ms, decrypter, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cryptoStream))
                        {
                            plaintext = sr.ReadToEnd();
                        }

                    }
                }
            }
            return plaintext;
        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            string plaintext = null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        private async Task<string> Decrypt(string cipherText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(_appSettings.EncryptKey);
                aesAlg.IV = Encoding.UTF8.GetBytes(_appSettings.EncryptIV);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherTextBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return await srDecrypt.ReadToEndAsync();
                        }
                    }
                }
            }
        }


        static byte[] Encrypt(string plainText)
        {
            byte[] encrypted;
            using (AesManaged aes = new AesManaged())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            return encrypted;
        }

        private CryptoStream EncryptStream(Stream responseStream)
        {
            Aes aes = GetEncryptionAlgorithm();
            ToBase64Transform base64Transform = new ToBase64Transform();
            CryptoStream base64EncodedStream = new CryptoStream(responseStream, base64Transform, CryptoStreamMode.Write);
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            CryptoStream cryptoStream = new CryptoStream(base64EncodedStream, encryptor, CryptoStreamMode.Write);
            return cryptoStream;
        }



        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }


        private Aes GetEncryptionAlgorithm()
        {
            Aes aes = Aes.Create();
            var secret_key = Encoding.UTF8.GetBytes(_appSettings.EncryptKey);
            var initialization_vector = Encoding.UTF8.GetBytes(_appSettings.EncryptIV);
            aes.Key = secret_key;
            aes.IV = initialization_vector;
            return aes;
        }



        private async Task<string> EncryptResponse(string plainText)
        {
            byte[] encrypted;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(_appSettings.EncryptKey);
                aesAlg.IV = Encoding.UTF8.GetBytes(_appSettings.EncryptIV);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            await swEncrypt.WriteAsync(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        private static List<string> GetExcludeURLList()
        {
            List<string> excludeURL = new()
            {
                "/api/Auth/authenticateUser",
                "/api/Auth/refreshtoken",
                "/api/Auth/revoke",
                "/api/RoleMenu/getAllAppUserRoles",
                "/api/RoleMenu/getAllAppUserRolesPagination",
                "/api/RoleMenu/getAppUserRolesById",
                "/api/RoleMenu/createUpdateAppUserRole",
                "/api/RoleMenu/deleteAppUserRole",
                "/api/RoleMenu/getAllAppUserMenuPagingWithSearch",
                "/api/RoleMenu/getAllAppUserMenuByUserId",
                "/api/RoleMenu/createUpdateAppUserMenu",
                "/api/RoleMenu/deleteAppUserMenu",
                "/api/RoleMenu/getAllParentMenus",
                "/api/RoleMenu/getAppUserRoleMenuInitialData",
                "/api/RoleMenu/getAllAppUserRoleMenusPagingWithSearch",
                "/api/User/createUpdateAppUser",
                "/api/User/getAllAppUserProfile",
                "/api/User/getAppUserProfileById",
                "/api/User/createUpdateAppUserProfile",
                "/api/User/deleteAppUserProfile"
            };
            return excludeURL;
        }
    }
}
