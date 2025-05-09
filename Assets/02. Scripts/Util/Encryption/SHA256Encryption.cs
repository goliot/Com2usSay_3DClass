using System.Security.Cryptography;
using System.Text;

public static class SHA256Encryption
{
    public static string Encrypt(string text)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            byte[] hashBytes = sha256.ComputeHash(bytes);

            // 바이트 배열을 16진수 문자열로 변환
            StringBuilder builder = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                builder.Append(b.ToString("x2")); // 소문자 16진수
            }

            return builder.ToString();
        }
    }
}