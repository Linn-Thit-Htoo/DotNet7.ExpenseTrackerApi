using System.Security.Cryptography;
using System.Text;

namespace DotNet7.ExpenseTrackerApi.Services;

public class AesService
{
    private readonly IConfiguration _configuration;

    public AesService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    #region Encryt Service
    public string EncryptString(string raw)
    {
        byte[] iv = new byte[16];
        byte[] array;
        using (System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(_configuration.GetSection("EncryptionKey").Value!);
            aes.IV = iv;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new MemoryStream();
            using CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write);
            using (StreamWriter streamWriter = new((Stream)cryptoStream))
            {
                streamWriter.Write(raw);
            }
            array = memoryStream.ToArray();
        }
        return Convert.ToBase64String(array);
    }
    #endregion

    #region Decrypt Service
    public string DecryptString(string encryptedString)
    {
        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(encryptedString);

        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_configuration.GetSection("EncryptionKey").Value!);
        aes.IV = iv;
        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using MemoryStream memoryStream = new(buffer);
        using CryptoStream cryptoStream = new((Stream)memoryStream, decryptor, CryptoStreamMode.Read);
        using StreamReader streamReader = new((Stream)cryptoStream);
        return streamReader.ReadToEnd();
    }
    #endregion
}
