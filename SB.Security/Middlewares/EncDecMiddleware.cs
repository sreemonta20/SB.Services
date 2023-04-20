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

        //        string decryptedRequest = await Decrypt(encryptedRequest);
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

        //        var decryptedRequest = await Decrypt(originalContent);

        //        //var decryptedRequest = DecryptStringFromBytes_Aes(Encoding.UTF8.GetBytes(originalContent),
        //        //    Encoding.UTF8.GetBytes(_appSettings.EncryptKey), Encoding.UTF8.GetBytes(_appSettings.EncryptIV));

        //        //My Custom Response Class
        //        var readingRequestBody = new RequstBodyReaderModel();
        //        //readingRequestBody.HttpVerb = request.Method;
        //        //readingRequestBody.RequestPath = request.Path;
        //        //readingRequestBody.RequestRawData = originalContent;
        //        readingRequestBody.RequestRawData = decryptedRequest;
        //        //readingRequestBody.Message = "Here I am Reading the request body";

        //        //converting my custom response class to jsontype
        //        var json = JsonConvert.SerializeObject(readingRequestBody);
        //        //Modifying existing stream
        //        var requestData = Encoding.UTF8.GetBytes(json);
        //        stream = new MemoryStream(requestData);
        //        request.Body = stream;
        //    }

        //    await _next(context);
        //}

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/User/login") && context.Request.Method == "POST")
            {
                var request = context.Request;
                var stream = request.Body;// At the begining it holding original request stream                    
                var originalReader = new StreamReader(stream);
                var originalContent = await originalReader.ReadToEndAsync(); // Reading first request

                var decryptedRequest = await DecryptObj(originalContent);
                var readingRequestBody = new RequstBodyReaderModel();
                readingRequestBody.RequestRawData = decryptedRequest;
                var json = JsonConvert.SerializeObject(readingRequestBody);
                var requestData =  Encoding.UTF8.GetBytes(json);
                stream = new MemoryStream(requestData);
                request.Body = stream;
            }

            await _next(context);
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
            return await streamReader.ReadToEndAsync() ;
        }

        public string Decrypt1(string value)
        {
            Aes aes = GetEncryptionAlgorithm();
            //ICryptoTransform decryptor = desSprovider.CreateDecryptor(key, iv);
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            Encoding utf = new UTF8Encoding();
            value = value.Replace(" ", "+").Replace("'", "");

            byte[] bEncrypt = Convert.FromBase64String(value);
            byte[] bDecrupt = decryptor.TransformFinalBlock(bEncrypt, 0, bEncrypt.Length);
            return utf.GetString(bDecrupt);
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


        //public async Task InvokeAsync(HttpContext httpContext)
        //{
        //    httpContext.Response.Body = EncryptStream(httpContext.Response.Body);
        //    httpContext.Request.Body = DecryptStream(httpContext.Request.Body);
        //    if (httpContext.Request.QueryString.HasValue)
        //    {
        //        string decryptedString = DecryptString(httpContext.Request.QueryString.Value.Substring(1));
        //        httpContext.Request.QueryString = new QueryString($"?{decryptedString}");
        //    }
        //    await _next(httpContext);
        //    await httpContext.Request.Body.DisposeAsync();
        //    await httpContext.Response.Body.DisposeAsync();
        //}

        //public async Task InvokeAsync(HttpContext context)
        //{
        //    context.Request.EnableBuffering();
        //    var api = new ApiRequest
        //    {
        //        HttpType = context.Request.Method,
        //        Query = context.Request.QueryString.Value,
        //        RequestUrl = context.Request.Path,
        //        RequestName = "",
        //        RequestIP = context.Request.Host.Value
        //    };

        //    var request = context.Request.Body;
        //    var response = context.Response.Body;

        //    try
        //    {
        //        //using (var newRequest = new MemoryStream())
        //        //{
        //        //    context.Request.Body = newRequest;
        //        //    using (var newResponse = new MemoryStream())
        //        //    {
        //        //        context.Response.Body = newResponse;
        //        //        using (var reader = new StreamReader(request))
        //        //        {
        //        //            api.Body = await reader.ReadToEndAsync();
        //        //            if (string.IsNullOrEmpty(api.Body))
        //        //                await _next.Invoke(context);
        //        //            api.Body = await Decrypt(api.Body);
        //        //        }
        //        //        using (var writer = new StreamWriter(newRequest))
        //        //        {
        //        //            await writer.WriteAsync(api.Body);
        //        //            await writer.FlushAsync();
        //        //            newRequest.Position = 0;
        //        //            context.Request.Body = newRequest;
        //        //            await _next(context);
        //        //        }

        //        //        using (var reader = new StreamReader(newResponse))
        //        //        {
        //        //            newResponse.Position = 0;
        //        //            api.ResponseBody = await reader.ReadToEndAsync();
        //        //            if (!string.IsNullOrWhiteSpace(api.ResponseBody))
        //        //            {
        //        //                api.ResponseBody = await EncryptResponse(api.ResponseBody);
        //        //            }
        //        //        }
        //        //        using (var writer = new StreamWriter(response))
        //        //        {
        //        //            await writer.WriteAsync(api.ResponseBody);
        //        //            await writer.FlushAsync();
        //        //        }
        //        //    }
        //        //}

        //        if (context.Request.Path.StartsWithSegments("/api/User/login") && context.Request.Method == "POST")
        //        {
        //            using (var newRequest = new MemoryStream())
        //            {
        //                context.Request.Body = newRequest;
        //                using (var newResponse = new MemoryStream())
        //                {
        //                    context.Response.Body = newResponse;
        //                    using (var reader = new StreamReader(request, Encoding.UTF8, true, 1024, true))
        //                    {
        //                        api.Body = await reader.ReadToEndAsync();
        //                        if (string.IsNullOrEmpty(api.Body))
        //                            await _next.Invoke(context);
        //                        api.Body = await Decrypt(api.Body);
        //                    }
        //                    using (var writer = new StreamWriter(newRequest))
        //                    {
        //                        await writer.WriteAsync(api.Body);
        //                        await writer.FlushAsync();
        //                        newRequest.Position = 0;
        //                        context.Request.Body = newRequest;
        //                        await _next(context);
        //                    }

        //                    using (var reader = new StreamReader(newResponse))
        //                    {
        //                        newResponse.Position = 0;
        //                        api.ResponseBody = await reader.ReadToEndAsync();
        //                        if (!string.IsNullOrWhiteSpace(api.ResponseBody))
        //                        {
        //                            api.ResponseBody = await EncryptResponse(api.ResponseBody);
        //                        }
        //                    }
        //                    using (var writer = new StreamWriter(response))
        //                    {
        //                        await writer.WriteAsync(api.ResponseBody);
        //                        await writer.FlushAsync();
        //                    }
        //                }
        //            }

        //        }
        //        await _next.Invoke(context);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    finally
        //    {
        //        context.Request.Body = request;
        //        context.Response.Body = response;
        //    }
        //    context.Response.OnCompleted(() =>
        //    {
        //        return Task.CompletedTask;
        //    });
        //}

        private CryptoStream EncryptStream(Stream responseStream)
        {
            Aes aes = GetEncryptionAlgorithm();
            ToBase64Transform base64Transform = new ToBase64Transform();
            CryptoStream base64EncodedStream = new CryptoStream(responseStream, base64Transform, CryptoStreamMode.Write);
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            CryptoStream cryptoStream = new CryptoStream(base64EncodedStream, encryptor, CryptoStreamMode.Write);
            return cryptoStream;
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

        private Aes GetEncryptionAlgorithm()
        {
            Aes aes = Aes.Create();
            var secret_key = Encoding.UTF8.GetBytes(_appSettings.EncryptKey);
            var initialization_vector = Encoding.UTF8.GetBytes(_appSettings.EncryptIV);
            aes.Key = secret_key;
            aes.IV = initialization_vector;
            return aes;
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

        private List<string> GetExcludeURLList()
        {
            List<string> excludeURL = new();
            //excludeURL.Add("/api/User/login");
            return excludeURL;
        }
    }
}
