using System.Security.Cryptography;
using System.Text;

namespace backendStd.Core.Util;

/// <summary>
/// MD5加密帮助类
/// </summary>
public static class MD5Helper
{
    /// <summary>
    /// MD5加密（32位小写）
    /// </summary>
    /// <param name="input">待加密字符串</param>
    /// <returns>加密后的字符串</returns>
    public static string Encrypt(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes).ToLower();
    }

    /// <summary>
    /// MD5加密（32位大写）
    /// </summary>
    /// <param name="input">待加密字符串</param>
    /// <returns>加密后的字符串</returns>
    public static string EncryptUpper(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// 验证MD5值
    /// </summary>
    /// <param name="input">原始字符串</param>
    /// <param name="hash">MD5值</param>
    /// <returns>是否匹配</returns>
    public static bool Verify(string input, string hash)
    {
        var encrypted = Encrypt(input);
        return string.Equals(encrypted, hash, StringComparison.OrdinalIgnoreCase);
    }
}
