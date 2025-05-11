using System.Security.Cryptography;
using System.Text;

namespace BookShop.Encryptor;

public static class EncryptionHelper
{
    private static readonly string EncryptionKey = "your-encryption-key-32bytes-long"; // Ключ для шифрования (32 байта для AES-256)

    // Шифрование данных
    public static string Encrypt(string data)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);

            // Генерация случайного IV для каждого шифрования
            aesAlg.GenerateIV(); 

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(data); // Записываем данные для шифрования

                // Сначала записываем IV, а затем зашифрованные данные
                byte[] iv = aesAlg.IV; 
                byte[] encryptedData = msEncrypt.ToArray();
                
                // Сохраняем IV перед зашифрованными данными, чтобы потом их можно было расшифровать
                byte[] result = new byte[iv.Length + encryptedData.Length];
                Array.Copy(iv, 0, result, 0, iv.Length);
                Array.Copy(encryptedData, 0, result, iv.Length, encryptedData.Length);
                
                return Convert.ToBase64String(result); // Возвращаем зашифрованные данные как строку
            }
        }
    }

    // Дешифрование данных
    public static string Decrypt(string encryptedData)
    {
        byte[] encryptedDataBytes = Convert.FromBase64String(encryptedData);

        // Разделяем IV и зашифрованные данные
        byte[] iv = new byte[16];
        byte[] data = new byte[encryptedDataBytes.Length - iv.Length];
        Array.Copy(encryptedDataBytes, iv, iv.Length);
        Array.Copy(encryptedDataBytes, iv.Length, data, 0, data.Length);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
            aesAlg.IV = iv;  // Устанавливаем IV, который использовался при шифровании

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(data))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd(); // Возвращаем дешифрованные данные
            }
        }
    }
}